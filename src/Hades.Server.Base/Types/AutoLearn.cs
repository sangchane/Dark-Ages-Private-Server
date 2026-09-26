#region

using System.Linq;
using Darkages.Network.ServerFormats;

#endregion

namespace Darkages.Types
{
    /// <summary>
    /// 레벨이 되면 그 직업의 기술·마법을 저절로 익힌다 — 사범에게 가지 않아도(사용자 결정 2026-09-26). 기준은 레벨·직업만
    /// (골드·재료·앞 단계 기술은 보지 않는다). 표 <see cref="Table" /> 는 5.99 밀레스마을 직업 사범 20명이 가르치는 레벨이다
    /// (<c>scripts/build-auto-learn.py</c>). 레벨업(<see cref="Monster.Levelup" />)과 로그인 때 부른다 — 로그인 때는 이미 넘은
    /// 레벨의 빠진 것을 한꺼번에 준다. 사범은 그대로 둔다(이미 있으면 "이미 이 스킬을 습득 하셧습니다.").
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

            var given = 0;

            foreach (var row in Table.Where(r => r.Path == aisling.Path && r.Level <= aisling.ExpLevel))
            {
                if (Knows(aisling, row.Name) || row.Instead.Any(name => Knows(aisling, name)))
                    continue;

                var learned = row.Skill ? Skill.GiveTo(client, row.Name) : Spell.GiveTo(client, row.Name);
                if (!learned)
                    continue;

                foreach (var old in row.Replaces)
                    Forget(aisling, old);

                // 5.99 사범의 말 "숏블레이드를 익히셧습니다." 앞부분 그대로.
                client.SendMessage(0x02, $"{row.Name}{Object(row.Name)} 익히셧습니다.");
                given++;
            }

            return given;
        }

        private static bool Knows(Aisling aisling, string name) =>
            aisling.SkillBook.Skills.Values.Any(s => s?.Template?.Name == name) || aisling.SpellBook.Has(name);

        private static void Forget(Aisling aisling, string name)
        {
            var skill = aisling.SkillBook.Skills.Values.FirstOrDefault(s => s?.Template?.Name == name);
            if (skill != null)
            {
                aisling.SkillBook.Remove((byte) skill.Slot);
                aisling.Client.Send(new ServerFormat2D((byte) skill.Slot));
            }

            var spell = aisling.SpellBook.Spells.Values.FirstOrDefault(s => s?.Template?.Name == name);
            if (spell != null)
            {
                aisling.SpellBook.Remove((byte) spell.Slot);
                aisling.Client.Send(new ServerFormat18((byte) spell.Slot));
            }
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
