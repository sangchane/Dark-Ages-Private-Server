using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 콜라마 — 5.99 `성직자(비전직).txt` 의 SPELL_콜라마 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손본 곳이 있어 `scripts/gen/ability/build-pack-abilities.py` 가 다시 만들지 않는다(아래 colama 줄).
    /// </remarks>
    [Script("콜라마", "5.99표")]
    public class SpellCF5CB77CB9C8 : SpellScript
    {
        public SpellCF5CB77CB9C8(Spell spell) : base(spell)
        {
        }

        public override void OnFailed(Sprite sprite, Sprite target)
        {
        }

        public override void OnSuccess(Sprite sprite, Sprite target)
        {
        }

        public override void OnUse(Sprite sprite, Sprite target)
        {
            var p = new Pack599(sprite, target);
            if (!p.Ready)
                return;
            V v_myid = 0;
            V v_target = 0;
            V v_type = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mapname")) == (V)((V)"OX퀴즈장"))))
            {
                p.Call("message", (V)3L, (V)"이벤트공간에서는 사용이불가능합니다.");
                return;
            }
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(v_type) != (V)((V)3L))))
            {
                return;
            }
            // 손본 곳(2026-10-05) — 팩(5.99·노바)은 콜라마를 체력 재생 10초(hprecovery)로 바꿔 두었다. 원작 콜라마는 방어 −10
            // (사용자) — 5.99 실행 파일에 따로 있는 `colama 대상, 초, 방어력량`(아이콘 94 = 이 마법 아이콘)을 쓴다. 원작 스크립트가
            // 남아 있지 않아 초는 짝 명령 벨라르모(belra 120초)와 같게 120 으로 둔다.
            p.Call("colama", v_target, (V)120L, (V)10L);
            p.Call("message", (V)3L, (V)"콜라마를 외웠습니다.");
            p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 콜라마를 외워주셨습니다.")));
            p.Call("game_sound", (V)8L, (V)0L);
        }
    }
}
