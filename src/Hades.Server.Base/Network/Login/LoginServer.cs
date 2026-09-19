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
                    client.SendMessageBox(0x03, $"There is no map configured for {aisling.AreaId}\0");
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
                client.SendMessageBox(0x03, "Character Already Exists.\0");
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
                        client.SendMessageBox(0x02, "Sorry, Incorrect Password.");
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
                        $"{format.Username} does not exist in this world. You can make this hero by clicking on 'Create'.");
                    return;
                }
            }
            catch (Exception ex)
            {
                ServerContext.Logger(ex.Message, Microsoft.Extensions.Logging.LogLevel.Error);
                ServerContext.Logger(ex.StackTrace, Microsoft.Extensions.Logging.LogLevel.Error);

                client.SendMessageBox(0x02,
                    $"{format.Username} is not supported by the new server. Please remake your character. This will not happen when the server goes to beta.");

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
                client.SendMessageBox(0x02, "A valid primary class must be selected.");
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
                client.SendMessageBox(0x02, "The selected class outfit is not configured.");
                client.CreateInfo = null;
                return;
            }

            // The mobile creator chooses the path before the first world entry, bypassing ClassChooser.
            // Keep the Monk's two requested opening techniques on that new-character path.  Aisling.Create
            // has already supplied Assail when the server configuration requires a base attack; do not add
            // Kick here as well, because this project maps 단각 to that same kick motion and the two would
            // become separate, duplicate attacks in the technique pane.
            if (!GiveMonkStarterSkills(template, path))
            {
                client.SendMessageBox(0x02, "The Monk starter skills are not configured.");
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
        /// Gives only the two deliberately selected Monk starters to a character created as a Monk.
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

            return Skill.GiveTo(aisling, "이형환위", 1)
                   && Skill.GiveTo(aisling, "단각", 1);
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
                client.SendMessageBox(0x02, "Incorrect Information provided.");
                return;
            }

            if (!Passwords.Verify(aisling.Password, format.Password, out _))
            {
                client.SendMessageBox(0x02, "Incorrect Information provided.");
                return;
            }

            if (string.IsNullOrEmpty(format.NewPassword) || format.NewPassword.Length < 3)
            {
                client.SendMessageBox(0x02, "new password not accepted.");
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
