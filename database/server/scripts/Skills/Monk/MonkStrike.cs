using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Network.Object;
using Darkages.Network.ServerFormats;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 무도가 기술이 쓰는 모양들. 5.99 서버팩 스크립트의 명령을 하데스 말로 옮긴 것이다.
    ///
    /// 팩의 체력 명령 셋은 뜻을 팩 자신이 보여 준다. <c>set_vital</c> 은 체력을 그 값으로 **맞춘다** —
    /// 팩이 <c>set_vital get_vita(@myid) - get_basevita(@myid)/100*10</c> 처럼 뺄셈을 직접 적어 넣고,
    /// <c>set_vital 1</c> 도 쓴다. <c>get_vita</c> 는 <c>set_vital</c> 이 쓰는 바로 그 자리(0xCC)를 읽으니
    /// **현재 체력**이고, <c>get_basevita</c> 는 <c>set_vital</c> 이 상한으로 함께 읽는 자리(0x90)라 **최대 체력**이다
    /// (`data/server-packs/extracted/5.99-server/script-command-evidence.json`).
    /// </summary>
    internal static class MonkStrike
    {
        /// <summary>
        /// 마력. 5.99: 모자라면 「사용하기에 마력량이적습니다」 하고 쓰지 않고, 넉넉하면 **표적을 찾기 전에** 먼저 뺀다.
        /// </summary>
        public static bool Spend(Sprite sprite, Skill skill, int mana)
        {
            if (!(sprite is Aisling aisling) || !skill.Ready)
                return false;

            if (aisling.CurrentMp < mana)
            {
                aisling.Client.SendMessage(0x02, $"사용하기에 마력량이적습니다. [필요마나 : {mana}이상]");
                return false;
            }

            aisling.CurrentMp -= mana;
            aisling.Client.SendStats(StatusFlags.StructB);
            return true;
        }

        /// <summary>
        /// 달마신공. 5.99: 현재 체력의 <paramref name="percent" />% 로 때리고, 맞혔으면 **내 체력을 그 값으로 맞춘다**.
        /// </summary>
        public static void UseVitality(Sprite sprite, Skill skill, int percent, byte motion)
        {
            if (!Begin(sprite, skill, out var aisling))
                return;

            var health = aisling.CurrentHp / 100 * percent;
            var hit = Hit(aisling, skill, aisling.GetInfront(), _ => health);

            if (hit)
                SetHealth(aisling, health);

            Swing(aisling, skill, motion, hit);
        }

        /// <summary>
        /// 구양신공. 5.99: 위·아래·왼쪽·오른쪽 네 칸을 현재 체력 × <paramref name="multiplier" /> 로 때리고
        /// (무도가가 아니면 3/4, 사람은 현재 체력 그대로), **맞히든 말든** 내 체력을 최대의
        /// <paramref name="remainPercent" />% 로 맞춘다.
        /// </summary>
        public static void UseCross(Sprite sprite, Skill skill, int multiplier, int remainPercent, byte motion)
        {
            if (!Begin(sprite, skill, out var aisling))
                return;

            var health = aisling.CurrentHp;
            var damage = health * multiplier;
            if (aisling.Path != Class.Monk)
                damage = damage / 4 * 3;

            var hit = Hit(aisling, skill, Around(aisling), target => target is Aisling ? health : damage);

            SetHealth(aisling, aisling.MaximumHp / 100 * remainPercent);
            Swing(aisling, skill, motion, hit);
        }

        /// <summary>
        /// 늑대의위상. 5.99: 반반의 확률로 (최대 체력 + 1) × <paramref name="low" /> 또는 × <paramref name="high" />.
        /// 팩은 큰 쪽의 딜레이를 3초로 줄이지만, 하데스는 스크립트가 끝난 뒤 템플릿의 쿨다운을 덮어써서 담지 못한다.
        /// </summary>
        public static void UseWolf(Sprite sprite, Skill skill, int low, int high, byte motion)
        {
            if (!Begin(sprite, skill, out var aisling))
                return;

            var damage = (aisling.MaximumHp + 1) * (Random.Shared.Next(2) == 0 ? low : high);
            Swing(aisling, skill, motion, Hit(aisling, skill, aisling.GetInfront(), _ => damage));
        }

        /// <summary>
        /// 마구때리기. 5.99: ((힘 + <paramref name="strength" />) + (지구력 + <paramref name="endurance" />)) × <paramref name="multiplier" />.
        /// </summary>
        public static void UseStrengthAndEndurance(
            Sprite sprite, Skill skill, int strength, int endurance, int multiplier, byte motion)
        {
            if (!Begin(sprite, skill, out var aisling))
                return;

            var damage = (aisling.Str + strength + aisling.Con + endurance) * multiplier;
            Swing(aisling, skill, motion, Hit(aisling, skill, aisling.GetInfront(), _ => damage));
        }

        /// <summary>
        /// 일음지(실명)·발경(빙결). 앞의 괴물에게 <paramref name="seconds" />초 동안 건다. 사람에게는
        /// <paramref name="players" /> 이고 결투 맵일 때만 — 팩이 <c>get_map_pk()</c> 를 볼 때만 사람을 친다.
        /// </summary>
        public static void Afflict(Sprite sprite, Skill skill, Debuff debuff, int seconds, bool players, byte motion)
        {
            if (!Begin(sprite, skill, out var aisling))
                return;

            var target = aisling.GetInfront(aisling)
                .Where(i => i.Attackable)
                .Where(i => i is Monster
                            || players && i is Aisling && i.Map.Flags.HasFlag(MapFlags.PlayerKill))
                .FirstOrDefault(i => !i.HasDebuff(debuff.Name));

            if (target != null)
            {
                // 하데스 디버프는 길이가 클래스에 박혀 있다(실명 7초·빙결 4초). 남은 시간은 `Length - Tick` 이다.
                debuff.Timer.Tick = debuff.Length - seconds;
                debuff.OnApplied(target, debuff);
                if (skill.Template.TargetAnimation > 0)
                    aisling.SendAnimation(skill.Template.TargetAnimation, target, aisling);
            }

            Swing(aisling, skill, motion, target != null);
        }

        /// <summary>
        /// 소수신공. 5.99 의 `sosusin` 은 캐릭터 칸 0x7C 를 켜고, 서버가 공격력을 계산할 때 그 칸이 켜져 있으면
        /// **공격력 +40%** 를 준다(Novaonline.exe — `Pack599.AttackPower`). <paramref name="seconds" />초 동안.
        /// </summary>
        public static void Empower(Sprite sprite, Skill skill, int seconds, byte motion, string said)
        {
            if (!Begin(sprite, skill, out var aisling))
                return;

            // 5.99 는 마력을 먼저 빼고 `sosusin` 이 이미 걸린 것을 거절한다. 거절 문구는 엔진 안에 있다.
            if (!Pack599.Pack599.Grant(aisling, "sosusin", seconds))
            {
                aisling.Client.SendMessage(0x02, "이미 걸려있습니다.");
                return;
            }

            if (skill.Template.TargetAnimation > 0)
                aisling.SendAnimation(skill.Template.TargetAnimation, aisling, aisling);
            aisling.Client.SendMessage(0x02, said);

            Swing(aisling, skill, motion, false);
        }

        /// <summary>
        /// 이형환위·허공답보. 5.99: 앞에 누가 있으면 보던 쪽으로 <paramref name="distance" />칸 건너뛰고 돌아선다.
        /// 내려설 칸이 막혔으면 쓰지 않는다. 허공답보는 돌아서서 넘어온 적을
        /// 공격력 × 공격력배율 + 최대 체력 × 최대체력배율 + 지구력 × 지구력배율 로 친다.
        /// </summary>
        public static void Step(
            Sprite sprite,
            Skill skill,
            int distance,
            int attackPercent = 0,
            int endurancePercent = 0,
            int maximumHealthPercent = 0)
        {
            if (!Begin(sprite, skill, out var aisling))
                return;

            if (!aisling.GetInfront(aisling).Any(i => i.Attackable))
                return;

            var (dx, dy) = aisling.Direction switch
            {
                0 => (0, -1),
                1 => (1, 0),
                2 => (0, 1),
                _ => (-1, 0)
            };
            var x = aisling.XPos + dx * distance;
            var y = aisling.YPos + dy * distance;

            // A leap must obey the same landing rule as a normal step: it cannot leave the map, land in a
            // wall, or overlap an occupied tile.  `IsWall` treats out-of-map coordinates as walls; the
            // TileGrid check keeps the collision exceptions (for example a summon) identical to Walk.
            if (aisling.Map.IsWall(x, y)
                || !aisling.Map.ObjectGrid[x, y].IsPassable(aisling, isAisling: true))
                return;

            aisling.XPos = x;
            aisling.YPos = y;
            aisling.Direction = (byte) ((aisling.Direction + 2) % 4);
            aisling.Client.Refresh();

            if (attackPercent == 0)
                return;

            var damage = Blow(aisling, skill, attackPercent, endurancePercent)
                         + aisling.MaximumHp * maximumHealthPercent / 100;
            Hit(aisling, skill, aisling.GetInfront(), _ => damage);
        }

        /// <summary>
        /// 무도가 한 방. 5.99 서버팩 스크립트의 모양을 그대로 쓴다 — 배율은 백분율이라 100 이 1배다.
        ///
        ///   피해 = 공격력 × 공격력배율 + 지구력 × 지구력배율
        ///
        /// 공격력은 5.99 서버가 계산하는 값 그대로다 — `Pack599.AttackPower` 에 식과 근거가 있다.
        /// </summary>
        public static void Use(
            Sprite sprite,
            Skill skill,
            int attackPercent,
            int endurancePercent,
            byte motion,
            Action<Sprite> onHit = null,
            int reach = 1,
            bool around = false)
        {
            if (!Begin(sprite, skill, out var aisling))
                return;

            // 5.99 가 칸을 직접 고르는 기술: 백보신권·무영신공은 보는 쪽 앞 세 칸, 선풍각·파천각은 둘레 네 칸.
            var hit = false;
            var targets = around ? Around(aisling).ToList() : aisling.GetInfront(reach);

            if (targets != null)
            {
                var damage = Blow(aisling, skill, attackPercent, endurancePercent);
                hit = Hit(aisling, skill, targets, _ => damage, onHit);
            }

            Swing(aisling, skill, motion, hit);
        }

        /// <summary>위·아래·왼쪽·오른쪽 붙은 네 칸에 선 것들.</summary>
        private static IEnumerable<Sprite> Around(Aisling aisling)
        {
            return aisling.GetObjects(aisling.Map,
                i => Math.Abs(i.XPos - aisling.XPos) + Math.Abs(i.YPos - aisling.YPos) == 1,
                ObjectManager.Get.Monsters | ObjectManager.Get.Aislings | ObjectManager.Get.Mundanes);
        }

        private static int Blow(Aisling aisling, Skill skill, int attackPercent, int endurancePercent)
        {
            // 5.99 의 `get_att_damage` — 장비·힘·무도가 레벨·버프로 서버가 계산하는 공격력이다.
            var blow = (int) Pack599.Pack599.AttackPower(aisling);

            var damage = blow * attackPercent / 100
                         + aisling.Con * endurancePercent / 100;

            // 기술 레벨이 오르면 1%씩 붙는다. 하데스가 쓰던 보정을 그대로 둔다.
            return damage + damage * (10 + skill.Level) / 100;
        }

        private static bool Begin(Sprite sprite, Skill skill, out Aisling aisling)
        {
            aisling = sprite as Aisling;
            if (aisling == null || !skill.Ready)
                return false;

            if (skill.Level < skill.Template.MaxLevel)
                aisling.Client.TrainSkill(skill);

            if (aisling.Invisible)
            {
                aisling.Invisible = false;
                aisling.Client.Refresh();
            }

            return true;
        }

        private static bool Hit(
            Aisling aisling,
            Skill skill,
            IEnumerable<Sprite> targets,
            Func<Sprite, int> damage,
            Action<Sprite> onHit = null)
        {
            var hit = false;

            foreach (var target in targets
                .Where(target => target != null)
                .Where(target => target.Serial != aisling.Serial)
                .Where(target => !(target is Money))
                .Where(target => target.Attackable))
            {
                target.ApplyDamage(aisling, damage(target), skill.Template.Sound);
                onHit?.Invoke(target);
                hit = true;

                if (target is Aisling player)
                {
                    player.Client.Aisling.Show(Scope.NearbyAislings,
                        new ServerFormat29((uint) aisling.Serial, (uint) target.Serial, byte.MinValue,
                            skill.Template.TargetAnimation, 100));
                    player.Client.Send(new ServerFormat08(player, StatusFlags.All));
                }

                if (target is Monster || target is Mundane || target is Aisling)
                    aisling.Show(Scope.NearbyAislings,
                        new ServerFormat29((uint) aisling.Serial, (uint) target.Serial,
                            skill.Template.TargetAnimation, 0, 100));
            }

            return hit;
        }

        private static void SetHealth(Aisling aisling, int health)
        {
            // 제 기술로 죽지는 않게 1 에서 멈춘다. 팩이 0 을 어떻게 다루는지는 모른다.
            aisling.CurrentHp = Math.Max(1, Math.Min(health, aisling.MaximumHp));
            aisling.Client.SendStats(StatusFlags.StructB);
        }

        private static void Swing(Aisling aisling, Skill skill, byte motion, bool hit)
        {
            if (!hit)
                aisling.Show(Scope.VeryNearbyAislings, new ServerFormat13(0, 0, skill.Template.Sound));

            aisling.Show(Scope.NearbyAislings, new ServerFormat1A
            {
                Serial = aisling.Serial,
                Number = motion,
                Speed = 30
            });
        }
    }
}
