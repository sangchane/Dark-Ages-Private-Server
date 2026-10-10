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

                // 없어진 맵(사라진 개인 사본 따위)은 게임 서버가 시작 자리로 보낸다(GameClient.Load) — 시작 맵까지 없을 때만 막는다.
                if (!ServerContext.GlobalMapCache.ContainsKey(aisling.AreaId)
                    && !ServerContext.GlobalMapCache.ContainsKey(ServerContext.Config.StartingMap))
                {
                    RecordLoginFailure(client, aisling.Username, "map_unavailable");
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

                EntryTickets.Issue(aisling.Username, client.Serial, client.Encryption.Parameters, DateTime.UtcNow);

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
            // 생태계 봇 이름은 같은 기계(봇 프로그램)만 만든다 — 밖에서 먼저 만들어 이름을 가로채지 못하게.
            if (!EcoBots.MayEnter(format.AislingUsername, RemoteAddress(client)))
            {
                client.SendMessageBox(0x03, "이미 등록된 계정입니다.\0");
                return;
            }

            // 카카오가 켜져 있으면 밖에서는 게임 표로만 만든다 — 주인은 0x04 에서 묶는다(사용자 2026-10-11).
            client.CreateKakaoId = null;
            if (KakaoLogin.Enabled)
            {
                var (allowed, kakaoId, refusal) = KakaoLogin.ForCreate(format.AislingPassword, Loopback(client), KakaoLogin.Checker);
                if (!allowed)
                {
                    client.SendMessageBox(0x03, refusal + "\0");
                    return;
                }
                client.CreateKakaoId = kakaoId;
            }

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

        private static IPAddress RemoteAddress(LoginClient client)
        {
            try
            {
                return (client.Socket?.RemoteEndPoint as IPEndPoint)?.Address;
            }
            catch (ObjectDisposedException)
            {
                return null;
            }
        }

        private static bool Loopback(LoginClient client) => RemoteAddress(client) is { } remote && ProxyHunt.IsLoopback(remote);

        private static bool Online(string name) =>
            ServerContext.Game.Clients.Any(i => i?.Aisling != null && i.Aisling.LoggedIn &&
                                                 string.Equals(i.Aisling.Username, name, StringComparison.OrdinalIgnoreCase));

        /// <summary>게임 서버가 들고 있는 그 사람 — 나가는 중이라도 아직 있으면 그것이 마지막에 저장된다.</summary>
        private static Aisling Live(string name) =>
            ServerContext.Game?.Clients.FirstOrDefault(i => i?.Aisling != null &&
                                                            string.Equals(i.Aisling.Username, name, StringComparison.OrdinalIgnoreCase))?.Aisling;

        private static bool Store(Aisling aisling) =>
            ServerContext.Config.DontSavePlayers || StorageManager.AislingBucket.TrySave(aisling);

        /// <summary>캐릭터 자물쇠 안에서 부른다. 저장하지 못하면 메모리도 되돌린다 — 다음 저장이 몰래 새 비밀번호를 쓰지 않게.</summary>
        private static bool StorePassword(Aisling aisling, string hash)
        {
            var old = aisling.Password;
            aisling.Password = hash;

            if (Store(aisling))
                return true;

            aisling.Password = old;
            return false;
        }

        /// <summary>캐릭터 자물쇠 안에서 부른다 — 옛 캐릭터에 카카오 주인을 묶는다. 저장하지 못하면 메모리도 되돌린다.</summary>
        private static bool StoreKakaoId(Aisling aisling, string kakaoId)
        {
            var old = aisling.KakaoId;
            aisling.KakaoId = kakaoId;

            if (Store(aisling))
                return true;

            aisling.KakaoId = old;
            return false;
        }

        private static void RecordLoginFailure(LoginClient client, string player, string reason)
        {
            string ip = "";
            try
            {
                if (client.Socket?.RemoteEndPoint is System.Net.IPEndPoint peer)
                    ip = peer.Address.IsIPv4MappedToIPv6 ? peer.Address.MapToIPv4().ToString() : peer.Address.ToString();
            }
            catch (ObjectDisposedException) { }
            Darkages.Network.Game.ActivitySession.LoginFailure(player, ip, reason);
        }

        protected override void Format03Handler(LoginClient client, ClientFormat03 format)
        {
            Aisling aisling = null;

            // 생태계 봇은 같은 기계(봇 프로그램)에서만 — 비밀번호가 새도 밖에서는 못 들어온다. 틀린 비밀번호와 같은 답.
            if (!EcoBots.MayEnter(format.Username, RemoteAddress(client)))
            {
                RecordLoginFailure(client, format.Username, "eco_remote");
                client.SendMessageBox(0x02, "비밀번호가 틀렸습니다.");
                return;
            }

            try
            {
                aisling = StorageManager.AislingBucket.Load(format.Username);

                if (aisling != null && ProxyHunt.Take(format.Username, format.Password, RemoteAddress(client),
                        Online(format.Username), DateTime.UtcNow, client.Serial) != null)
                {
                    // 대신 사냥 — 같은 기계의 대리 프로그램이 일회용 열쇠로 들어온다. 접속해 있지 않을 때만 열리므로 밀어낼 이도 없다.
                    LoginAsAisling(client, aisling);
                    return;
                }

                if (aisling != null)
                {
                    // 카카오(사용자 2026-10-11) — 밖에서는 게임 표로만, 옛 캐릭터는 옛 비밀번호 한 번으로 주인을 묶는다. 같은 기계는 평문 그대로.
                    var kakao = KakaoLogin.Enabled
                        ? KakaoLogin.Decide(aisling.KakaoId, format.Password, Loopback(client), KakaoLogin.Checker)
                        : (KakaoLogin.Verdict.PasswordAsBefore, null, null);

                    if (kakao.Verdict == KakaoLogin.Verdict.Refuse)
                    {
                        RecordLoginFailure(client, format.Username, "kakao");
                        client.SendMessageBox(0x02, kakao.Message);
                        return;
                    }

                    bool needsRehash = false;
                    if (kakao.Verdict != KakaoLogin.Verdict.Enter)
                    {
                        string password = kakao.Verdict == KakaoLogin.Verdict.Link ? KakaoLogin.Split(format.Password).OldPassword : format.Password;
                        if (!Passwords.Verify(aisling.Password, password, out needsRehash))
                        {
                            RecordLoginFailure(client, format.Username, "password");
                            client.SendMessageBox(0x02, "비밀번호가 틀렸습니다.");
                            return;
                        }
                    }

                    if (kakao.Verdict == KakaoLogin.Verdict.Link)
                    {
                        // 옛 비밀번호가 맞았다. 자물쇠 안에서 다시 읽어 그새 다른 카카오 계정이 묶지 않았는지 본 뒤 묶는다 —
                        // 접속 중이면 그 사람(메모리)에(StorePassword 와 같은 까닭).
                        string refused = null;
                        lock (AislingStorage.LockFor(aisling.Username))
                        {
                            var owner = Live(aisling.Username) ?? StorageManager.AislingBucket.Load(aisling.Username);
                            if (owner == null || !string.IsNullOrEmpty(owner.KakaoId) && owner.KakaoId != kakao.KakaoId)
                                refused = KakaoLogin.OtherOwner;
                            else if (!StoreKakaoId(owner, kakao.KakaoId))
                                refused = "캐릭터를 저장하지 못했습니다. 잠시 뒤 다시 해 주십시오.";
                        }

                        if (refused != null)
                        {
                            client.SendMessageBox(0x02, refused);
                            return;
                        }

                        aisling.KakaoId = kakao.KakaoId;
                        needsRehash = false; // 비밀번호 칸이 「표|옛비밀번호」라 그대로 해시하면 안 된다 — 밖에서는 더 쓰지 않는다
                    }

                    // The account was made before passwords were hashed. It has just proved the password,
                    // so this is the one moment the plain value is in hand — store the hash and never again.
                    if (needsRehash)
                    {
                        var hash = Passwords.Hash(format.Password); // 느린 해시는 캐릭터 자물쇠 밖에서

                        lock (AislingStorage.LockFor(aisling.Username))
                            StorePassword(Live(aisling.Username) ?? aisling, hash);
                    }
                }
                else
                {
                    RecordLoginFailure(client, format.Username, "account");
                    client.SendMessageBox(0x02,
                        $"{format.Username}: 없는 계정 입니다.");
                    return;
                }
            }
            catch (Exception ex)
            {
                ServerContext.Error(ex);
                RecordLoginFailure(client, format.Username, "read_failure");

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
                    // 다시 들어오는 앱이 밀어내는 접속은 대리에게 넘기지 않는다.
                    obj.ProxyArm = null;
                    obj.Aisling?.Remove(true);
                    obj.Server.ClientDisconnected(obj);
                }
            }

            // 앱이 돌아왔다 — 기다리던 대신 사냥은 없던 일로.
            ProxyHunt.Cancel(format.Username);

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
            // 게임 표로 만들었으면 주인을 묶고 비밀번호는 아무도 모르는 값 — 표를 비밀번호로 남기지 않는다.
            template.Password = Passwords.Hash(client.CreateKakaoId == null ? client.CreateInfo.AislingPassword : Guid.NewGuid().ToString("N"));
            template.KakaoId = client.CreateKakaoId;
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

            client.CreateInfo = null;

            if (!Store(template))
            {
                client.SendMessageBox(0x02, "캐릭터를 저장하지 못했습니다. 잠시 뒤 다시 해 주십시오.");
                return;
            }

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
            // 생태계 봇 비밀번호는 같은 기계에서만 바꾼다 — 밖에서 바꾸면 봇 프로그램 전체가 못 들어온다.
            if (!EcoBots.MayEnter(format.Username, RemoteAddress(client)))
            {
                client.SendMessageBox(0x02, "계정을 바르게 적어주시길 바랍니다.");
                return;
            }

            // 카카오가 켜져 있으면 밖에서는 바꿀 비밀번호가 없다(사용자 2026-10-11) — 같은 기계(봇)만.
            if (KakaoLogin.Enabled && !Loopback(client))
            {
                client.SendMessageBox(0x02, KakaoLogin.NoPassword);
                return;
            }

            string refused;
            var seen = (Live(format.Username) ?? StorageManager.AislingBucket.Load(format.Username))?.Password;

            // 옛 비밀번호 확인·새 해시(느리다)는 캐릭터 자물쇠 밖에서 — 안에서 하면 이름만 아는 남이 바꾸기를 연달아 보내 그 사람의
            // 저장·은행·경매(같은 자물쇠)를 붙잡을 수 있었다(보안 리뷰 2026-10-08). 자물쇠는 확인을 통과한 뒤에만 잡는다.
            if (seen == null || !Passwords.Verify(seen, format.Password, out _))
                refused = "계정을 바르게 적어주시길 바랍니다.";
            else if (string.IsNullOrEmpty(format.NewPassword) || format.NewPassword.Length < 3)
                refused = "암호를 바르게 적어주시길 바랍니다.";
            else
            {
                var hash = Passwords.Hash(format.NewPassword);

                // 접속 중이면 그 사람(메모리)의 비밀번호를 바꿔 저장한다. 디스크에서 따로 읽어 바꾸면 다음 저장이 옛 비밀번호로 덮고,
                // 따로 읽은 것을 통째로 쓰면 마지막 저장 뒤의 진행이 옛 상태로 돌아간다(리뷰 2026-10-08 #2). 캐릭터 자물쇠라 저장과 엇갈리지 않는다.
                lock (AislingStorage.LockFor(format.Username))
                {
                    var aisling = Live(format.Username) ?? StorageManager.AislingBucket.Load(format.Username);

                    if (aisling == null || aisling.Password != seen)
                        refused = "계정을 바르게 적어주시길 바랍니다."; // 확인하는 사이 바뀌었다
                    else if (!StorePassword(aisling, hash))
                        refused = "암호를 저장하지 못했습니다. 잠시 뒤 다시 해 주십시오.";
                    else
                        refused = null;
                }
            }

            client.SendMessageBox(refused == null ? (byte) 0x00 : (byte) 0x02, refused ?? "\0");
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
