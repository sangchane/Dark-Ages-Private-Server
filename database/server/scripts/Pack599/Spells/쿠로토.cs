using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 쿠로토 — 5.99 `공통스킬.txt` 의 SPELL_쿠로토 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("쿠로토", "5.99표")]
    public class SpellCFE0B85CD1A0 : SpellScript
    {
        public SpellCFE0B85CD1A0(Spell spell) : base(spell)
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
            V v_hill = 0;
            V v_myid = 0;
            V v_target = 0;
            V v_type = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)(((V)(((V)(p.Call("get_basemana", v_myid)) / (V)((V)100L))) * (V)((V)3L))))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 3%]");
                return;
            }
            p.Call("manal_del", ((V)(((V)(p.Call("get_basemana", v_myid)) / (V)((V)100L))) * (V)((V)3L)));
            // 5.99 는 위즈×5(최대 300) — 무도가는 위즈가 낮아 25 남짓이었다. 사용자 기억(2026-09-25): "쿠로토 체력 100 정도 회복".
            v_hill = (V)100L;
            v_type = p.Call("istype", v_myid);
            // 손본 곳(2026-09-24, 사용자 "도복 입어도 쿠로토 모션 있어" · "쿠로토 빠르다") — 생성기가 다시 만들면 되돌아간다.
            // 5.99 표의 136(마법사 시전)은 원작 클라이언트가 skill.tbl 8번 줄 ST 옷(마법사 옷)에만 그린다(Legend.exe 2005
            // 0x4e1161~0x4e1171). 도복(착용이미지 3)은 거기 없어 무도가는 몸이 안 움직였다. 혼든 팩의 쿠로토는 `motion 6, 30`
            // (손 들기 — 옷을 가리지 않는 03 파일)이고, 하데스 Aisling.Cast 도 사제 128 · 마법사 136 · 그 밖은 6 이다.
            // 링은 5.99 의 75 에서 20%씩 두 번 느리게(75 / 0.8 / 0.8 ≈ 117). 몸은 앱이 모든 동작에 30% 를 더 걸어 링보다 늦었다
            // (사용자 2026-09-25: "모션이 이펙트에 비해 느리다") — 117 / 1.3 ≈ 90 을 보내 링과 같은 빠르기로 맞춘다.
            var kurotoPath = (sprite as Aisling)?.Path;
            p.Call("motion", kurotoPath == Class.Priest ? (V)128L : kurotoPath == Class.Wizard ? (V)136L : (V)6L, (V)90L);
            p.Call("effect", v_target, (V)4L, (V)0L, (V)75L);  // 노바 이펙트(5.99: 4, 0, 속도 117)
            p.Call("game_sound", (V)8L, (V)0L);
            p.Call("set_vita", v_myid, ((V)(p.Call("get_vita", v_myid)) + (V)(v_hill)));
        }
    }
}
