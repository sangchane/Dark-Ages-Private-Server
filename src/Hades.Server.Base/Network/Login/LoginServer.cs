#region

using Darkages.Network.ClientFormats;
using Darkages.Network.ServerFormats;
using Darkages.Security;
using Darkages.Storage;
using Darkages.Types;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

#endregion

namespace Darkages.Network.Login
{
    public class LoginServer : NetworkServer<LoginClient>
    {
        // The five base-class armour pairs are the 5.99 imported level-one templates.  Their Image values
        // are precisely the first clothing values in skill.tbl's ST lists: warrior 2, rogue 4, wizard 6,
        // priest 5, monk 3.  Equipping one real item (instead of faking an appearance byte) means saves,
        // reconnects, inventory and the motion gate all agree.
        private static readonly IReadOnlyDictionary<Class, (string Male, string Female)> StarterOutfits =
            new Dictionary<Class, (string Male, string Female)>
            {
                [Class.Warrior] = ("레더튜닉", "튜닉"),
                [Class.Rogue] = ("스카웃튜닉", "꼬뜨"),
                [Class.Wizard] = ("후드로브", "매직스커트"),
                [Class.Priest] = ("셍즈", "로브"),
                [Class.Monk] = ("도복", "연무복")
            };

        public LoginServer(int capacity)
            : base(capacity)
        {
            MServerTable = MServerTable.FromFile("MServerTable.xml");
            Notification = Notification.FromFile("notification.txt");
        }

        public static MServerTable MServerTable { get; set; }
        public static Notification Notification { get; set; }

        public override void ClientConnected(LoginClient client)
        {
            client.Send(new ServerFormat7E());
        }

        public void LoginAsAisling(LoginClient client, Aisling aisling)
        {
            if (aisling != null)
            {

                if (!ServerContext.GlobalMapCache.ContainsKey(aisling.AreaId))
                {
                    client.SendMessageBox(0x03, $"{aisling.AreaId}번 맵이 준비되어 있지 않습니다.\0");
                    return;
                }

                var redirect = new Redirect
                {
                    Serial = Convert.ToString(client.Serial),
                    Salt = Encoding.UTF8.GetString(client.Encryption.Parameters.Salt),
                    Seed = Convert.ToString(client.Encryption.Parameters.Seed),
                    Name = aisling.Username,
                };

                ServerContext.Redirects.Add(redirect.Name.ToLower());

                client.SendMessageBox(0x00, "\0");
                client.Send(new ServerFormat03
                {
                    EndPoint = new IPEndPoint(Address, ServerContext.Config.SERVER_PORT),
                    Redirect = redirect
                });
            }
        }

        protected override void Format00Handler(LoginClient client, ClientFormat00 format)
        {
            if (ServerContext.Config.UseLobby)
                if (format.Version == ServerContext.Config.ClientVersion)
                    client.Send(new ServerFormat00
                    {
                        Type = 0x00,
                        Hash = MServerTable.Hash,
                        Parameters = client.Encryption.Parameters
                    });


            if (ServerContext.Config.DevMode)
                if (ServerContext.Config.GameMasters != null)
                    foreach (var unused in ServerContext.Config.GameMasters)
                    {
                        var aisling = StorageManager.AislingBucket.Load(unused);

                        if (aisling != null)
                        {
                            LoginAsAisling(client, aisling);
                            break;
                        }
                    }
        }

        protected override void Format02Handler(LoginClient client, ClientFormat02 format)
        {
            client.CreateInfo = format;

            var aisling = StorageManager.AislingBucket.Load(format.AislingUsername);

            if (aisling == null)
            {
                client.SendMessageBox(0x00, "\0");
            }
            else
            {
                client.SendMessageBox(0x03, "이미 등록된 계정입니다.\0");
                client.CreateInfo = null;
            }
        }

        protected override void Format03Handler(LoginClient client, ClientFormat03 format)
        {
            Aisling aisling = null;

            try
            {
                aisling = StorageManager.AislingBucket.Load(format.Username);

                if (aisling != null)
                {
                    if (!Passwords.Verify(aisling.Password, format.Password, out bool needsRehash))
                    {
                        client.SendMessageBox(0x02, "비밀번호가 틀렸습니다.");
                        return;
                    }

                    // The account was made before passwords were hashed. It has just proved the password,
                    // so this is the one moment the plain value is in hand — store the hash and never again.
                    if (needsRehash)
                    {
                        aisling.Password = Passwords.Hash(format.Password);
                        StorageManager.AislingBucket.Save(aisling);
                    }
                }
                else
                {
                    client.SendMessageBox(0x02,
                        $"{format.Username}: 없는 계정 입니다.");
                    return;
                }
            }
            catch (Exception ex)
            {
                ServerContext.Logger(ex.Message, Microsoft.Extensions.Logging.LogLevel.Error);
                ServerContext.Logger(ex.StackTrace, Microsoft.Extensions.Logging.LogLevel.Error);

                client.SendMessageBox(0x02,
                    $"{format.Username}: 이 서버에서 읽을 수 없는 캐릭터입니다. 새로 만들어 주십시오.");

                return;
            }

            if (!ServerContext.Config.MultiUserLogin)
            {
                var aislings = ServerContext.Game.Clients.Where(i =>
                    i?.Aisling != null && i.Aisling.LoggedIn &&
                    i.Aisling.Username.ToLower() == format.Username.ToLower());

                foreach (var obj in aislings)
                {
                    obj.Aisling?.Remove(true);
                    obj.Server.ClientDisconnected(obj);
                }
            }

            LoginAsAisling(client, aisling);
        }

        protected override void Format04Handler(LoginClient client, ClientFormat04 format)
        {
            if (client.CreateInfo == null)
            {
                ClientDisconnected(client);
                return;
            }

            if (format.Path < (byte)Class.Warrior || format.Path > (byte)Class.Monk)
            {
                client.SendMessageBox(0x02, "직업을 골라 주십시오.");
                client.CreateInfo = null;
                return;
            }

            var path = (Class)format.Path;
            var template = Aisling.Create();
            template.Display = (BodySprite) (format.Gender * 16);
            template.Username = client.CreateInfo.AislingUsername;
            template.Password = Passwords.Hash(client.CreateInfo.AislingPassword);
            template.Gender = (Gender) format.Gender;
            template.HairColor = format.HairColor;
            template.HairStyle = format.HairStyle;
            template.Path = path;

            if (!EquipStarterOutfit(template, path, template.Gender))
            {
                client.SendMessageBox(0x02, "고른 직업의 옷이 준비되어 있지 않습니다.");
                client.CreateInfo = null;
                return;
            }

            // The mobile creator chooses the path before the first world entry, bypassing ClassChooser.
            // A new Monk starts with exactly 이형환위 · 단각 and the spell 쿠로토 (user, 2026-09-27).
            // Aisling.Create has already supplied Assail when the server configuration requires a base attack —
            // it stays, because the attack button (0x13) only swings the Assail-type skills in the book — and
            // the configured starter spell, which the Monk gives up for 쿠로토.  Do not add Kick here as well:
            // this project maps 단각 to that same kick motion and the two would become duplicate attacks.
            if (!GiveMonkStarterSkills(template, path))
            {
                client.SendMessageBox(0x02, "무도가의 첫 기술이 준비되어 있지 않습니다.");
                client.CreateInfo = null;
                return;
            }

            StorageManager.AislingBucket.Save(template);
            client.CreateInfo = null;
            client.SendMessageBox(0x00, "\0");
        }

        /// <summary>
        /// Puts a real, equipped level-one class outfit in the character JSON before its first login.
        /// EquipmentManager needs a live GameClient to send packets, so creation records the slot directly;
        /// GameClient.LoadEquipment restores its template/scripts and sends ServerFormat37 on first entry.
        /// </summary>
        private static bool EquipStarterOutfit(Aisling aisling, Class path, Gender gender)
        {
            if (!StarterOutfits.TryGetValue(path, out (string Male, string Female) names))
            {
                return false;
            }

            string name = gender == Gender.Female ? names.Female : names.Male;

            if (!ServerContext.GlobalItemTemplateCache.TryGetValue(name, out var outfitTemplate))
            {
                return false;
            }

            Item outfit = Item.Create(aisling, outfitTemplate);

            if (outfit?.Template == null || outfit.Template.EquipmentSlot != ItemSlots.Armor)
            {
                return false;
            }

            aisling.EquipmentManager.Equipment[ItemSlots.Armor] =
                new EquipmentSlot(ItemSlots.Armor, outfit);

            return true;
        }

        /// <summary>
        /// Gives only the deliberately selected Monk starters to a character created as a Monk: the techniques
        /// 이형환위 · 단각 beside the base attack, and 쿠로토 in place of the configured starter spell.
        /// <see cref="Skill.GiveTo(Aisling, string, int)"/> also loads the template's script and assigns the
        /// appropriate skill-pane slots before the character is serialized, so the same entries return on
        /// every later login.
        /// </summary>
        private static bool GiveMonkStarterSkills(Aisling aisling, Class path)
        {
            if (path != Class.Monk)
            {
                return true;
            }

            aisling.SpellBook = new SpellBook();

            return Skill.GiveTo(aisling, "이형환위", 1)
                   && Skill.GiveTo(aisling, "단각", 1)
                   && Spell.GiveTo(aisling, "쿠로토", 1);
        }

        protected override void Format0BHandler(LoginClient client, ClientFormat0B format)
        {
            RemoveClient(client);
        }

        protected override void Format10Handler(LoginClient client, ClientFormat10 format)
        {
            client.Encryption.Parameters = format.Parameters;
            client.Send(new ServerFormat60
            {
                Type = 0x00,
                Hash = Notification.Hash
            });
        }

        protected override void Format26Handler(LoginClient client, ClientFormat26 format)
        {
            var aisling = StorageManager.AislingBucket.Load(format.Username);

            if (aisling == null)
            {
                client.SendMessageBox(0x02, "계정을 바르게 적어주시길 바랍니다.");
                return;
            }

            if (!Passwords.Verify(aisling.Password, format.Password, out _))
            {
                client.SendMessageBox(0x02, "계정을 바르게 적어주시길 바랍니다.");
                return;
            }

            if (string.IsNullOrEmpty(format.NewPassword) || format.NewPassword.Length < 3)
            {
                client.SendMessageBox(0x02, "암호를 바르게 적어주시길 바랍니다.");
                return;
            }

            aisling.Password = Passwords.Hash(format.NewPassword);
            StorageManager.AislingBucket.Save(aisling);

            client.SendMessageBox(0x00, "\0");
        }

        protected override void Format4BHandler(LoginClient client, ClientFormat4B format)
        {
            client.Send(new ServerFormat60
            {
                Type = 0x01,
                Size = Notification.Size,
                Data = Notification.Data
            });
        }

        protected override void Format57Handler(LoginClient client, ClientFormat57 format)
        {
            if (format.Type == 0x00)
            {
                var redirect = new Redirect
                {
                    Serial = Convert.ToString(client.Serial),
                    Salt = Encoding.UTF8.GetString(client.Encryption.Parameters.Salt),
                    Seed = Convert.ToString(client.Encryption.Parameters.Seed),
                    Name = "socket[" + client.Serial + "]"
                };

                client.Send(new ServerFormat03
                {
                    EndPoint = new IPEndPoint(MServerTable.Servers[0].Address, MServerTable.Servers[0].Port),
                    Redirect = redirect
                });
            }
            else
            {
                client.Send(new ServerFormat56
                {
                    Size = MServerTable.Size,
                    Data = MServerTable.Data
                });
            }
        }

        protected override void Format68Handler(LoginClient client, ClientFormat68 format)
        {
            client.Send(new ServerFormat66());
        }

        protected override void Format7BHandler(LoginClient client, ClientFormat7B format)
        {
            if (format.Type == 0x00)
            {
                ServerContext.Logger("Client Requested Metafile: {0}", format.Name);

                client.Send(new ServerFormat6F
                {
                    Type = 0x00,
                    Name = format.Name
                });
            }

            if (format.Type == 0x01)
                client.Send(new ServerFormat6F
                {
                    Type = 0x01
                });
        }
    }
}
