// 손으로 고치지 말 것. `python3 scripts/build-auto-learn.py --쓰기` 가 다시 만든다.
// 근거: 노바 팩 1차 스킬상인(`db/script/스킬배우기.txt` 메뉴 글의 레벨)과 전직 첫 기술(`npc_script.txt`).
// 치울 것(Withdrawn): 5.99 밀레스마을 직업 사범 20명(`database/server/scripts/Pack599/Npcs`)이 가르치는데 노바 1차 목록에 없는 것.
namespace Darkages.Types
{
    public static partial class AutoLearn
    {
        /// <summary>
        /// 직업 · 배우는 레벨 · 기술이면 true(마법이면 false) · 템플릿 이름 · 이 중 하나라도 있으면 주지 않는다(윗 기술 — 사범의
        /// "상위스킬" 거절, 승급하며 지운 것) · 주면서 지운다(사범의 `spell_del`). 노바 1차 스킬상인 기준(사용자 2026-09-27).
        /// </summary>
        public static readonly (Class Path, int Level, bool Skill, string Name, string[] Instead, string[] Replaces)[] Table =
        {
            (Class.Warrior, 1, true, "숏블레이드", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 전직
            (Class.Warrior, 11, true, "윈드블레이드", new string[] { "룬블레이드" }, System.Array.Empty<string>()), // 노바 전사스킬상인
            (Class.Warrior, 11, false, "쿠로토", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 전사스킬상인
            (Class.Warrior, 41, true, "메가블레이드", new string[] { "스톰블레이드" }, System.Array.Empty<string>()), // 노바 전사스킬상인
            (Class.Warrior, 41, true, "바투", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 전사스킬상인
            (Class.Warrior, 71, true, "투핸드어택", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 전사스킬상인
            (Class.Warrior, 99, true, "매드소울", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 전사스킬상인
            (Class.Warrior, 99, true, "완전방어", new string[] { "완전방어(UP)" }, System.Array.Empty<string>()), // 노바 전사스킬상인
            (Class.Warrior, 99, true, "크래셔", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 전사스킬상인
            (Class.Rogue, 1, true, "찌르기", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 전직
            (Class.Rogue, 11, true, "센스몬스터", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 도적스킬상인
            (Class.Rogue, 11, true, "찔러휘비기", new string[] { "마구찌르기" }, System.Array.Empty<string>()), // 노바 도적스킬상인
            (Class.Rogue, 11, false, "쿠로토", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 도적스킬상인
            (Class.Rogue, 41, true, "두번찌르기", new string[] { "슬레쉬" }, System.Array.Empty<string>()), // 노바 도적스킬상인
            (Class.Rogue, 41, true, "센스", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 도적스킬상인
            (Class.Rogue, 41, true, "품뒤져보기", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 도적스킬상인
            (Class.Rogue, 41, false, "하이드", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 도적스킬상인
            (Class.Rogue, 71, true, "습격", new string[] { "습격진" }, System.Array.Empty<string>()), // 노바 도적스킬상인
            (Class.Rogue, 99, true, "암살격", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 도적스킬상인
            (Class.Wizard, 1, false, "마레노", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 전직
            (Class.Wizard, 11, false, "렌토", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 마법사스킬상인
            (Class.Wizard, 11, false, "수페라마레나", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 마법사스킬상인
            (Class.Wizard, 11, false, "쿠로토", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 마법사스킬상인
            (Class.Wizard, 41, false, "나르콜리", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 마법사스킬상인
            (Class.Wizard, 41, false, "마레누스", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 마법사스킬상인
            (Class.Wizard, 41, false, "바르도", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 마법사스킬상인
            (Class.Wizard, 41, false, "엑스마레나", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 마법사스킬상인
            (Class.Wizard, 71, false, "데프레코", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 마법사스킬상인
            (Class.Wizard, 71, false, "마네나로", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 마법사스킬상인
            (Class.Wizard, 71, false, "마레네라", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 마법사스킬상인
            (Class.Wizard, 99, false, "라그나로크", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 마법사스킬상인
            (Class.Wizard, 99, false, "세멜리아", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 마법사스킬상인
            (Class.Wizard, 99, false, "프라보", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 마법사스킬상인
            (Class.Priest, 1, false, "쿠로", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 전직
            (Class.Priest, 11, false, "벨라르모", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 11, false, "에나르마", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 11, false, "이모탈", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 11, false, "쿠라노", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 11, false, "쿠러스", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 11, false, "홀리볼트", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 41, false, "디나르콜리", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 41, false, "디베노모", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 41, false, "디소루마", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 41, false, "수페라벨라르모", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 41, false, "콜라마", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 41, false, "쿠라노소", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 41, false, "쿠라누스", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 71, false, "리베라토", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 71, false, "수페라쿠라노", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 71, false, "쿠라네라", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 71, false, "호르라마", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 99, false, "엑스쿠라네라", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 99, false, "엑스쿠라노", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Priest, 99, false, "코마디아", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 성직자스킬상인
            (Class.Monk, 11, true, "단각", new string[] { "연천단각" }, System.Array.Empty<string>()), // 노바 무도가스킬상인
            (Class.Monk, 11, true, "이형환위", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 무도가스킬상인
            (Class.Monk, 11, false, "쿠로토", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 무도가스킬상인
            (Class.Monk, 41, true, "붕각", new string[] { "붕신선각" }, System.Array.Empty<string>()), // 노바 무도가스킬상인
            (Class.Monk, 41, true, "일음지", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 무도가스킬상인
            (Class.Monk, 71, true, "발경", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 무도가스킬상인
            (Class.Monk, 71, true, "선풍각", new string[] { "파천각" }, System.Array.Empty<string>()), // 노바 무도가스킬상인
            (Class.Monk, 71, false, "금강불괴", new string[] { "자기보호" }, System.Array.Empty<string>()), // 노바 무도가스킬상인
            (Class.Monk, 71, false, "장풍", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 무도가스킬상인
            (Class.Monk, 99, true, "구양신공", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 무도가스킬상인
            (Class.Monk, 99, true, "달마신공", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 무도가스킬상인
            (Class.Monk, 99, false, "다라밀공", System.Array.Empty<string>(), System.Array.Empty<string>()), // 노바 무도가스킬상인
        };

        /// <summary>
        /// 5.99 사범만 가르치던 것 — 이 직업 캐릭터의 창에서 치운다(사용자 2026-09-27 「5.99 에만 있는 것은 뺀다」).
        /// 이 목록에 있는 이름만 지운다. 직업 · 기술이면 true(마법이면 false) · 템플릿 이름.
        /// </summary>
        public static readonly (Class Path, bool Skill, string Name)[] Withdrawn =
        {
            (Class.Warrior, true, "내려치기"), // 5.99 가렌 16레벨
            (Class.Warrior, true, "타겟어택"), // 5.99 가렌4 83레벨
            (Class.Warrior, true, "피닉스모드"), // 5.99 가렌3 55레벨
            (Class.Warrior, true, "휘두르기"), // 5.99 가렌3 62레벨
            (Class.Warrior, false, "파워단련"), // 5.99 가렌 11레벨
            (Class.Rogue, true, "만개표창"), // 5.99 이블린3 71레벨
            (Class.Rogue, true, "소매치기"), // 5.99 이블린3 62레벨
            (Class.Rogue, true, "슬레쉬"), // 5.99 이블린4 87레벨
            (Class.Rogue, true, "아무네지아"), // 5.99 이블린 15레벨
            (Class.Rogue, true, "원기지옥"), // 5.99 이블린3 74레벨
            (Class.Rogue, true, "차크라어택"), // 5.99 이블린4 83레벨
            (Class.Rogue, true, "표창던지기"), // 5.99 이블린2 35레벨
            (Class.Rogue, false, "마구찌르기"), // 5.99 이블린 11레벨
            (Class.Rogue, false, "명중률향상(Lev1)"), // 5.99 이블린 11레벨
            (Class.Rogue, false, "명중률향상(Lev2)"), // 5.99 이블린2 31레벨
            (Class.Rogue, false, "암살"), // 5.99 이블린4 99레벨
            (Class.Wizard, false, "딜루메니"), // 5.99 럭스4 87레벨
            (Class.Wizard, false, "원소이해력"), // 5.99 럭스 11레벨
            (Class.Wizard, false, "침묵"), // 5.99 럭스3 74레벨
            (Class.Wizard, false, "콘푸지오"), // 5.99 럭스 15레벨
            (Class.Priest, false, "신성력강화"), // 5.99 소라카 11레벨
            (Class.Priest, false, "안티매직"), // 5.99 소라카2 50레벨
            (Class.Priest, false, "일루메나"), // 5.99 소라카4 81레벨
            (Class.Priest, false, "홀리랜서"), // 5.99 소라카3 73레벨
            (Class.Monk, true, "백보신권"), // 5.99 리신4 87레벨
            (Class.Monk, true, "소수신공"), // 5.99 리신3 70레벨
            (Class.Monk, true, "양의신권"), // 5.99 리신2 21레벨
            (Class.Monk, true, "연환포"), // 5.99 리신3 74레벨
            (Class.Monk, false, "일루메나"), // 5.99 리신4 81레벨
            (Class.Monk, false, "주먹단련"), // 5.99 리신 11레벨
            (Class.Monk, false, "쿠라노토"), // 5.99 리신3 55레벨
        };
    }
}
