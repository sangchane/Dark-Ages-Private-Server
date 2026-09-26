#region

using System.Linq;
using Darkages.Network.ServerFormats;

#endregion

namespace Darkages.Types
{
    /// <summary>
    /// 레벨이 되면 그 직업의 기술·마법을 저절로 익힌다 — 사범에게 가지 않아도(사용자 결정 2026-09-26). 기준은 레벨·직업만
    /// (골드·재료·앞 단계 기술은 보지 않는다). 표 <see cref="Table" /> 는 노바 팩 1차 스킬상인이 가르치는 레벨과 전직 첫 기술이다
    /// (사용자 결정 2026-09-27, <c>scripts/build-auto-learn.py</c>). 레벨업(<see cref="Monster.Levelup" />)과 로그인 때 부른다 — 로그인
    /// 때는 이미 넘은 레벨의 빠진 것을 한꺼번에 준다. 5.99 사범만 가르치던 것(<see cref="Withdrawn" />)은 먼저 창에서 치운다 —
    /// 그 목록에 있는 이름만. 사범은 그대로 둔다(이미 있으면 "이미 이 스킬을 습득 하셧습니다.").
    /// 동료 봇은 부를 때 제 목록(<see cref="Companions.PriestSpells" />)을 받으므로 건드리지 않는다.
    /// </summary>
    public static partial class AutoLearn
    {
        /// <summary>이 캐릭터의 직업·레벨에서 배울 수 있는데 아직 없는 것을 준다. 준 개수를 돌려준다.</summary>
        public static int Catchup(Aisling aisling)
        {
            var client = aisling?.Client;
            if (client == null || Companions.IsBot(aisling.Username))
                return 0;

            // 치우는 것이 먼저다 — 5.99 의 마구찌르기가 남아 있으면 노바 찔러휘비기의 「있으면 안 줌」에 걸린다.
            foreach (var row in Withdrawn.Where(r => r.Path == aisling.Path))
            {
                if (Forget(aisling, row.Name, row.Skill))
                    client.SendMessage(0x02, $"{row.Name}{Object(row.Name)} 잊었습니다.");
            }

            var given = 0;

            foreach (var row in Table.Where(r => r.Path == aisling.Path && r.Level <= aisling.ExpLevel))
            {
                if (Knows(aisling, row.Name) || row.Instead.Any(name => Knows(aisling, name)))
                    continue;

                var learned = row.Skill ? Skill.GiveTo(client, row.Name) : Spell.GiveTo(client, row.Name);
                if (!learned)
                    continue;

                foreach (var old in row.Replaces)
                {
                    Forget(aisling, old, skill: true);
                    Forget(aisling, old, skill: false);
                }

                // 5.99 사범의 말 "숏블레이드를 익히셧습니다." 앞부분 그대로.
                client.SendMessage(0x02, $"{row.Name}{Object(row.Name)} 익히셧습니다.");
                given++;
            }

            return given;
        }

        private static bool Knows(Aisling aisling, string name) =>
            aisling.SkillBook.Skills.Values.Any(s => s?.Template?.Name == name) || aisling.SpellBook.Has(name);

        /// <summary>그 이름의 기술(<paramref name="skill" />) 또는 마법을 창에서 지운다. 지웠으면 true.</summary>
        private static bool Forget(Aisling aisling, string name, bool skill)
        {
            if (skill)
            {
                var known = aisling.SkillBook.Skills.Values.FirstOrDefault(s => s?.Template?.Name == name);
                if (known == null)
                    return false;
                aisling.SkillBook.Remove((byte) known.Slot);
                aisling.Client.Send(new ServerFormat2D((byte) known.Slot));
                return true;
            }

            var spell = aisling.SpellBook.Spells.Values.FirstOrDefault(s => s?.Template?.Name == name);
            if (spell == null)
                return false;
            aisling.SpellBook.Remove((byte) spell.Slot);
            aisling.Client.Send(new ServerFormat18((byte) spell.Slot));
            return true;
        }

        /// <summary>받침이 있으면 "을", 없으면 "를". 한글로 끝나지 않으면(`명중률향상(Lev1)`) "을(를)".</summary>
        private static string Object(string name)
        {
            var last = name[^1];
            if (last < '가' || last > '힣')
                return "을(를)";
            return (last - '가') % 28 == 0 ? "를" : "을";
        }
    }
}
