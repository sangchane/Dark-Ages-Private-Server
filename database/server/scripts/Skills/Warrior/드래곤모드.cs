using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    // 이 파일은 손으로 쓴 것이다 — Pack599 의 도우미(Pack599·V)를 쓰려고 같은 이름공간에 둔다.
    /// <summary>
    /// 드래곤모드 — 커뮤니티 팩 `db/script/[Skill,Spell]/전사기술.txt` 의 SKILL_드래곤모드 를 옮긴 것(5.99 에는 없다). 피닉스모드와
    /// 같은 꼴로 체력 9% 를 쓰고 60초 공격력 강화. 전사 41레벨에 배우고 91레벨 피닉스모드가 지운다(사용자 2026-10-05).
    /// </summary>
    [Script("드래곤모드", "Warrior")]
    public class DragonMode : SkillScript
    {
        public DragonMode(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
        }

        public override void OnUse(Sprite sprite)
        {
            if (!Skill.Ready)
                return;

            var p = new Pack599(sprite, null);
            if (!p.Ready)
                return;

            p.Train(Skill);
            V v_myid = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_vita", v_myid)) < (V)((V)100L))))
            {
                p.Call("message", (V)3L, (V)"체력이 부족합니다.");
                return;
            }
            p.Call("set_vital", ((V)(p.Call("get_vita", v_myid)) - (V)(((V)(((V)(p.Call("get_vita", v_myid)) / (V)((V)100L))) * (V)((V)9L)))));
            p.Call("skill_delay", (V)20L);
            if (V.T(((V)(p.Call("phoenix", v_myid, (V)60L)) == (V)((V)1L))))
            {
                p.Call("game_sound", (V)18L, (V)100L);
                p.Call("message", (V)3L, (V)"드래곤을 외웠습니다.");
                p.Call("effect", v_myid, (V)34L, (V)0L, (V)100L);
                p.Call("motion", (V)129L, (V)25L);
            }
        }
    }
}
