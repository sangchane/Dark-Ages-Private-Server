using System.Collections.Generic;
using Darkages.Types;

namespace Darkages.Network.ServerFormats
{
    /// <summary>
    /// 봇(동료) 사이의 소식. **우리 확장이다, 원작 패킷이 아니다.** 원작 4.51 클라이언트의 패킷 점프표
    /// (<c>0x510a54</c>, <c>data/disassembly/findings.json</c>)는 0x03~0x4B 뿐이고 하데스도 0x5E 를 <c>Undefined.cs</c> 의
    /// 빈 자리로만 두었다 — 0x5C·0x5D 와 같은 방식으로 고른 번호다. 첫 바이트(종류)가 뒤의 모양을 정한다. 모두 빅엔디언.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>1 <see cref="Master" /> — 봇에게만: serial(4) · 이름. "네 주인은 이 사람". serial 0 이면 풀려났다.</item>
    /// <item>2 <see cref="Companion" /> — 부른 사람에게: serial(4) · 이름. "네 봇은 이것". serial 0 이면 없다.</item>
    /// <item>3 <see cref="Statuses" /> — 봇에게만, 1초마다: serial(4, 주인 또는 봇 자신) · 개수(1) ·
    /// [이름(StringA) · 남은 초(2) · 해로움(1)]×개수. 하데스 버프·디버프(<c>Sprite.Buffs</c>·<c>Debuffs</c>, 이름·Length−Tick)와
    /// 5.99 스크립트가 거는 상태(<see cref="TimedStates" /> — horrama·enare 등)를 함께 싣는다. 봇 자신 것도 보내는 까닭: 5.99
    /// 상태(호르라마·에나르마)는 원작 상태 아이콘(0x3A)으로 오지 않아, 봇이 제 버프를 알 길이 이것뿐이다.</item>
    /// <item>4 <see cref="Vitals" /> — 부른 사람에게, 1초마다: serial(4, 봇) · 체력 %(1) · 마력 %(1). 파티원 체력을 알리는 원작 패킷이 없다.</item>
    /// <item>5 <see cref="Gear" /> — 부른 사람에게, 부를 때와 바뀔 때: serial(4, 봇) · 장비 개수(1) · [0x37 몸 그대로 — 자리(1) ·
    /// 그림(2) · 3(1) · 이름 · 보이는 이름 · 내구(4) · 최대 내구(4)]×개수 · 가방 물건 개수(1) · [이름 · 그림(2) · 개수(2)]×개수.</item>
    /// </list>
    /// 귓속말(0x19)로 알리지 않은 까닭: 귓속말은 아무 사람이나 봇에게 보낼 수 있어 주인을 속일 수 있다.
    /// </remarks>
    public class ServerFormat5E : NetworkFormat
    {
        public const byte Master = 1;
        public const byte Companion = 2;
        public const byte Statuses = 3;
        public const byte Vitals = 4;
        public const byte Gear = 5;

        public ServerFormat5E()
        {
            Secured = true;
            Command = 0x5E;
        }

        public ServerFormat5E(byte kind, int serial, string name) : this()
        {
            Kind = kind;
            Serial = serial;
            Name = name;
        }

        public byte Kind { get; set; }
        public int Serial { get; set; }
        public string Name { get; set; }

        public IReadOnlyList<(string Name, int Seconds, bool Harmful)> Listed { get; set; }
        public byte HealthPercent { get; set; }
        public byte ManaPercent { get; set; }
        public IReadOnlyList<(byte Place, Item Item)> Worn { get; set; }
        public IReadOnlyList<Item> Carried { get; set; }

        public static ServerFormat5E Status(int serial, IReadOnlyList<(string Name, int Seconds, bool Harmful)> listed) =>
            new ServerFormat5E { Kind = Statuses, Serial = serial, Listed = listed };

        public static ServerFormat5E Life(int serial, byte health, byte mana) =>
            new ServerFormat5E { Kind = Vitals, Serial = serial, HealthPercent = health, ManaPercent = mana };

        public static ServerFormat5E Kit(int serial, IReadOnlyList<(byte Place, Item Item)> worn, IReadOnlyList<Item> carried) =>
            new ServerFormat5E { Kind = Gear, Serial = serial, Worn = worn, Carried = carried };

        public override void Serialize(NetworkPacketReader reader)
        {
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Kind);
            writer.Write(Serial);

            switch (Kind)
            {
                case Statuses:
                    writer.Write((byte) Listed.Count);
                    foreach (var (name, seconds, harmful) in Listed)
                    {
                        writer.WriteStringA(name ?? string.Empty);
                        writer.Write((ushort) System.Math.Clamp(seconds, 0, ushort.MaxValue));
                        writer.Write((byte) (harmful ? 1 : 0));
                    }

                    break;

                case Vitals:
                    writer.Write(HealthPercent);
                    writer.Write(ManaPercent);
                    break;

                case Gear:
                    writer.Write((byte) Worn.Count);
                    foreach (var (place, item) in Worn)
                    {
                        writer.Write(place);
                        writer.Write(item.DisplayImage);
                        writer.Write((byte) 0x03);
                        writer.WriteStringA(item.Template.Name);
                        writer.WriteStringA(item.DisplayName);
                        writer.Write(item.Durability);
                        writer.Write(item.Template.MaxDurability);
                    }

                    writer.Write((byte) Carried.Count);
                    foreach (var item in Carried)
                    {
                        writer.WriteStringA(item.Template.Name);
                        writer.Write(item.DisplayImage);
                        writer.Write((ushort) System.Math.Max(1, (int) item.Stacks));
                    }

                    break;

                default:
                    writer.WriteStringA(Name ?? string.Empty);
                    break;
            }
        }
    }
}
