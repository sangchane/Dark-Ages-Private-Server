#region

using System.Collections.Generic;
using System.ComponentModel;

#endregion

namespace Darkages.Types
{
    public class SkillTemplate : Template
    {
        /// <summary>무도가 한 방의 공격력 배율, 백분율(100 이 1배). `MonkStrike.Use` 가 읽는다.</summary>
        public int AttackPercent { get; set; }

        public Buff Buff { get; set; }
        /// <summary>현재 체력 비율, 백분율. 달마신공이 현재 체력의 몇 % 로 치는지.</summary>
        public int CurrentHealthPercent { get; set; }

        public double Cooldown { get; set; }
        public Debuff Debuff { get; set; }
        /// <summary>건너뛰는 칸 수. 이형환위·허공답보.</summary>
        public int Distance { get; set; }

        /// <summary>무도가 한 방의 지구력 배율, 백분율. 0 이면 지구력은 더하지 않는다.</summary>
        public int EndurancePercent { get; set; }

        public string FailMessage { get; set; }
        /// <summary>체력에 곱하는 수. 구양신공(현재 체력 ×)·늑대의위상(최대 체력 + 1 의 작은 쪽 ×).</summary>
        public int HealthMultiplier { get; set; }

        /// <summary>늑대의위상이 반반으로 고르는 큰 쪽 배수.</summary>
        public int HighHealthMultiplier { get; set; }

        public byte Icon { get; set; }

        public List<LearningPredicate> LearningRequirements { get; set; } = new List<LearningPredicate>();
        public double LevelRate { get; set; }
        /// <summary>쓸 때 빼는 마력. 0 이면 마력을 보지 않는다. `MonkStrike.Spend` 가 읽는다.</summary>
        public int ManaCost { get; set; }

        /// <summary>최대 체력 비율, 백분율. 허공답보는 피해에 더하고, 구양신공은 쓴 뒤 내 체력을 이 값으로 맞춘다.</summary>
        public int MaximumHealthPercent { get; set; }

        public int MaxLevel { get; set; }
        public ushort MissAnimation { get; set; }
        public string NpcKey { get; set; }
        public Pane Pane { get; set; }
        public PostQualifer PostQualifers { get; set; }
        public LearningPredicate Prerequisites { get; set; }
        /// <summary>걸어 두는 초. 일음지(실명)·발경(빙결)·소수신공.</summary>
        public int Seconds { get; set; }

        public string ScriptName { get; set; }
        /// <summary>마구때리기 — ((힘 + StrengthBonus) + (지구력 + EnduranceBonus)) × StatMultiplier.</summary>
        public int StatMultiplier { get; set; }

        public int StrengthBonus { get; set; }

        public int EnduranceBonus { get; set; }

        public byte Sound { get; set; }
        public ushort TargetAnimation { get; set; }
        public ushort TargetAnimationSpeed { get; set; } = 100;
        public Tier TierLevel { get; set; }
        public SkillScope Type { get; set; }

        public override string[] GetMetaData()
        {
            return Prerequisites?.MetaData;
        }
    }
}
