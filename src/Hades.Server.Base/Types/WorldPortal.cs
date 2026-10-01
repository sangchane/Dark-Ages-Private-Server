using System.Collections.Generic;

namespace Darkages.Types
{
    public class WorldPortal
    {
        public Warp Destination { get; set; }

        public string DisplayName { get; set; }

        public short PointX { get; set; }

        public short PointY { get; set; }

        // 이 사냥터의 구역 — 모바일 월드맵에서 사냥터를 누르면 그 아래 목록으로 보이고, 고르면 바로 그 구역으로 간다(사용자 2026-10-02).
        public List<Warp> Zones { get; set; }
    }
}