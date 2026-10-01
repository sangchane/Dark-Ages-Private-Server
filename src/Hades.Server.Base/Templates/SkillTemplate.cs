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
        public double Cooldown { get; set; }
        public Debuff Debuff { get; set; }
        /// <summary>무도가 한 방의 지구력 배율, 백분율. 0 이면 지구력은 더하지 않는다.</summary>
        public int EndurancePercent { get; set; }

        public string FailMessage { get; set; }
        public byte Icon { get; set; }

        public List<LearningPredicate> LearningRequirements { get; set; } = new List<LearningPredicate>();
        public double LevelRate { get; set; }
        public int MaxLevel { get; set; }
        public ushort MissAnimation { get; set; }
        public string NpcKey { get; set; }
        public Pane Pane { get; set; }
        public PostQualifer PostQualifers { get; set; }
        public LearningPredicate Prerequisites { get; set; }
        public string ScriptName { get; set; }
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
