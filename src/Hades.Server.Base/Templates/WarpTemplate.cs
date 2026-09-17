#region

using System.Collections.Generic;
using Darkages.Templates;
using Newtonsoft.Json.Converters;

#endregion

namespace Darkages.Types
{
    public class WarpTemplate : Template
    {
        public WarpTemplate()
        {
            Activations = new List<Warp>();
        }

          public int ActivationMapId { get; set; }
        public List<Warp> Activations { get; set; }
          public byte LevelRequired { get; set; }

        /// <summary>
        /// 이 레벨을 넘으면 못 들어간다. 0 은 제한 없음. 5.99 워프 줄의 마지막 칸(노비스 사냥터 22 · 포테의숲 51)이고,
        /// 99 는 제한이 없다는 뜻이라 0 으로 들어온다(tools/pack-import/import.py).
        /// </summary>
        public byte LevelMaximum { get; set; }

        public Warp To { get; set; }
        public int WarpRadius { get; set; }

        public WarpType WarpType { get; set; }

        public int WorldResetWarpId { get; set; }
        public int WorldTransionWarpId { get; set; }

        public override string[] GetMetaData()
        {
            return new[]
            {
                ""
            };
        }
    }
}