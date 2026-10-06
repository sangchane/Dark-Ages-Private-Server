namespace Darkages.Network.ClientFormats
{
    /// <summary>
    /// 봇(동료)에게 하는 말 — 우리 앱의 말이다. 원작에는 없다(사용자, 2026-09-26).
    /// </summary>
    /// <remarks>
    /// 첫 바이트가 종류: 1 부르기 · 0 보내기 · 2 주기(내 가방 칸(1) · 개수(2) — 장비면 봇에게 입히고, 겹치는 물건(포션)이면
    /// 그만큼 봇 가방으로, 개수 0 은 다) · 3 벗기기(봇 장비 자리(1) — 내 가방으로) · 4 혼수인 봇 깨우기(몸 없음) · 5 봇이 혼수인 주인 깨우기(몸 없음 — 봇 계정만, 주인 바로 옆에서)
    /// · 6 봇 탭에서 고른 것(2026-10-03) — 마법사 비트(1 렌토 · 2 나르콜리 · 4 바르도 · 8 데프레코 · 16 프라보) · 성직자 비트(1 디나르콜리 ·
    /// 2 디소루마 · 4 호르라마 · 8 에나르마) · 회복 셀렉트 · 파티 회복 셀렉트(0 자동 · k 번째까지 · 255 끄기), 넷 다 한 바이트.
    /// 0xF1 인 까닭: 원작 클라이언트는 0x80 넘는 명령을 보내지 않고(0xF0 월드맵 열기와 같은 근거), 하데스는 0xF1 을
    /// <c>Undefined.cs</c> 의 빈 자리로만 두었다 — 0xF0 다음 빈 번호다.
    /// </remarks>
    public class ClientFormatF1 : NetworkFormat
    {
        public const byte Dismiss = 0;
        public const byte Call = 1;
        public const byte Give = 2;
        public const byte TakeOff = 3;
        public const byte Wake = 4;
        public const byte WakeMaster = 5;
        public const byte Magic = 6;

        /// <summary>대신 사냥 맡김 설정(2026-10-05) — u16 길이 + UTF-8 JSON. 길이 0 이면 지움(<see cref="Darkages.Types.ProxyHunt" />).</summary>
        public const byte Proxy = 7;

        /// <summary>생태계 봇 순간이동(2026-10-06) — 맵 u16 · x · y. 생태계 봇이 같은 기계에서 보낸 것만 듣는다(<see cref="Darkages.Types.EcoBots" />).</summary>
        public const byte EcoMove = 8;

        /// <summary>생태계 성직자가 혼수인 같은 그룹 파티원을 깨운다(2026-10-07) — 대상 serial u32(<see cref="Darkages.Types.EcoBots.Wake" />).</summary>
        public const byte EcoWake = 9;

        public ClientFormatF1()
        {
            Secured = true;
            Command = 0xF1;
        }

        public byte Kind { get; set; }
        public byte Slot { get; set; }
        public ushort Count { get; set; }

        /// <summary>종류 6 — 마법사 비트 · 성직자 비트 · 회복 셀렉트 · 파티 회복 셀렉트 · 따라가기 거리(0 기본, 옛 앱은 안 보내 0).</summary>
        public byte[] Orders { get; set; }

        /// <summary>종류 7 — 맡김 설정 JSON 바이트.</summary>
        public byte[] Payload { get; set; }

        /// <summary>종류 8 — 갈 맵·칸.</summary>
        public uint Target { get; set; }

        public ushort Map { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            Kind = reader.ReadByte();

            if (Kind == Give || Kind == TakeOff)
                Slot = reader.ReadByte();

            if (Kind == Magic)
            {
                Orders = new[] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), (byte) 0 };
                // 여섯째(2026-10-05) — 주인 체력이 몇 % 이하면 회복. 옛 앱은 보내지 않는다(0 = 봇 기본).
                if (reader.GetCanRead())
                    Orders[5] = reader.ReadByte();
            }

            if (Kind == Give)
                Count = reader.ReadUInt16();

            if (Kind == Proxy)
            {
                // GetCanRead 는 두 바이트 이상 남아야 참이라 마지막 바이트를 놓친다 — 길이만큼 그대로 읽는다(모자라면 0, JSON 이 깨져 버려진다).
                Payload = reader.ReadBytes(reader.ReadUInt16());
            }

            if (Kind == EcoWake)
                Target = reader.ReadUInt32();

            if (Kind == EcoMove)
            {
                Map = reader.ReadUInt16();
                X = reader.ReadByte();
                Y = reader.ReadByte();
            }
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}
