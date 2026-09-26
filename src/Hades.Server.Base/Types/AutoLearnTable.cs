// 손으로 고치지 말 것. `python3 scripts/build-auto-learn.py --쓰기` 가 다시 만든다.
// 근거: 5.99 밀레스마을 직업 사범 20명의 스크립트(`database/server/scripts/Pack599/Npcs`) — 직업 검사와 갈래마다의 레벨 검사.
namespace Darkages.Types
{
    public static partial class AutoLearn
    {
        /// <summary>
        /// 직업 · 배우는 레벨 · 기술이면 true(마법이면 false) · 템플릿 이름 · 이 중 하나라도 있으면 주지 않는다(윗 기술 — 사범의
        /// "상위스킬" 거절, 승급하며 지운 것) · 주면서 지운다(사범의 `spell_del`).
        /// </summary>
        public static readonly (Class Path, int Level, bool Skill, string Name, string[] Instead, string[] Replaces)[] Table =
        {
            (Class.Warrior, 5, true, "숏블레이드", System.Array.Empty<string>(), System.Array.Empty<string>()), // 가렌
            (Class.Warrior, 11, true, "윈드블레이드", new string[] { "룬블레이드" }, System.Array.Empty<string>()), // 가렌
            (Class.Warrior, 11, false, "파워단련", System.Array.Empty<string>(), System.Array.Empty<string>()), // 가렌
            (Class.Warrior, 16, true, "내려치기", System.Array.Empty<string>(), System.Array.Empty<string>()), // 가렌
            (Class.Warrior, 21, true, "메가블레이드", new string[] { "스톰블레이드" }, System.Array.Empty<string>()), // 가렌2
            (Class.Warrior, 21, true, "바투", System.Array.Empty<string>(), System.Array.Empty<string>()), // 가렌2
            (Class.Warrior, 41, true, "완전방어", System.Array.Empty<string>(), System.Array.Empty<string>()), // 가렌2
            (Class.Warrior, 50, true, "매드소울", new string[] { "매드소울진" }, System.Array.Empty<string>()), // 가렌2
            (Class.Warrior, 55, true, "피닉스모드", System.Array.Empty<string>(), System.Array.Empty<string>()), // 가렌3
            (Class.Warrior, 62, true, "휘두르기", System.Array.Empty<string>(), System.Array.Empty<string>()), // 가렌3
            (Class.Warrior, 71, true, "투핸드어택", System.Array.Empty<string>(), System.Array.Empty<string>()), // 가렌3
            (Class.Warrior, 83, true, "타겟어택", System.Array.Empty<string>(), System.Array.Empty<string>()), // 가렌4
            (Class.Warrior, 99, true, "크래셔", new string[] { "데빌크래셔" }, System.Array.Empty<string>()), // 가렌4
            (Class.Rogue, 11, false, "마구찌르기", System.Array.Empty<string>(), System.Array.Empty<string>()), // 이블린
            (Class.Rogue, 11, false, "명중률향상(Lev1)", new string[] { "명중률향상(Lev2)" }, System.Array.Empty<string>()), // 이블린
            (Class.Rogue, 15, true, "아무네지아", System.Array.Empty<string>(), System.Array.Empty<string>()), // 이블린
            (Class.Rogue, 30, true, "습격", new string[] { "습격진" }, System.Array.Empty<string>()), // 이블린2
            (Class.Rogue, 31, false, "명중률향상(Lev2)", System.Array.Empty<string>(), new string[] { "명중률향상(Lev1)" }), // 이블린2
            (Class.Rogue, 35, true, "표창던지기", System.Array.Empty<string>(), System.Array.Empty<string>()), // 이블린2
            (Class.Rogue, 41, false, "하이드", System.Array.Empty<string>(), System.Array.Empty<string>()), // 이블린2
            (Class.Rogue, 50, true, "암살격", System.Array.Empty<string>(), System.Array.Empty<string>()), // 이블린2
            (Class.Rogue, 57, true, "찔러휘비기", new string[] { "후벼쑤시기" }, System.Array.Empty<string>()), // 이블린3
            (Class.Rogue, 62, true, "소매치기", System.Array.Empty<string>(), System.Array.Empty<string>()), // 이블린3
            (Class.Rogue, 71, true, "만개표창", System.Array.Empty<string>(), System.Array.Empty<string>()), // 이블린3
            (Class.Rogue, 74, true, "원기지옥", System.Array.Empty<string>(), System.Array.Empty<string>()), // 이블린3
            (Class.Rogue, 83, true, "차크라어택", System.Array.Empty<string>(), System.Array.Empty<string>()), // 이블린4
            (Class.Rogue, 87, true, "슬레쉬", new string[] { "블로우" }, System.Array.Empty<string>()), // 이블린4
            (Class.Rogue, 99, false, "암살", System.Array.Empty<string>(), System.Array.Empty<string>()), // 이블린4
            (Class.Wizard, 11, false, "렌토", System.Array.Empty<string>(), System.Array.Empty<string>()), // 럭스
            (Class.Wizard, 11, false, "원소이해력", System.Array.Empty<string>(), System.Array.Empty<string>()), // 럭스
            (Class.Wizard, 11, false, "쿠로토", System.Array.Empty<string>(), System.Array.Empty<string>()), // 럭스
            (Class.Wizard, 15, false, "콘푸지오", System.Array.Empty<string>(), System.Array.Empty<string>()), // 럭스
            (Class.Wizard, 21, false, "바르도", System.Array.Empty<string>(), System.Array.Empty<string>()), // 럭스2
            (Class.Wizard, 21, false, "수페라마레나", System.Array.Empty<string>(), System.Array.Empty<string>()), // 럭스2
            (Class.Wizard, 31, false, "마레누스", System.Array.Empty<string>(), System.Array.Empty<string>()), // 럭스2
            (Class.Wizard, 41, false, "나르콜리", System.Array.Empty<string>(), System.Array.Empty<string>()), // 럭스2
            (Class.Wizard, 50, false, "세멜리아", System.Array.Empty<string>(), System.Array.Empty<string>()), // 럭스2
            (Class.Wizard, 55, false, "데프레코", System.Array.Empty<string>(), System.Array.Empty<string>()), // 럭스3
            (Class.Wizard, 71, false, "마네나로", System.Array.Empty<string>(), System.Array.Empty<string>()), // 럭스3
            (Class.Wizard, 74, false, "침묵", System.Array.Empty<string>(), System.Array.Empty<string>()), // 럭스3
            (Class.Wizard, 83, false, "프라보", System.Array.Empty<string>(), System.Array.Empty<string>()), // 럭스4
            (Class.Wizard, 87, false, "딜루메니", System.Array.Empty<string>(), System.Array.Empty<string>()), // 럭스4
            (Class.Wizard, 99, false, "라그나로크", System.Array.Empty<string>(), System.Array.Empty<string>()), // 럭스4
            (Class.Priest, 11, false, "신성력강화", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카
            (Class.Priest, 11, false, "쿠러스", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카
            (Class.Priest, 15, false, "호르라마", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카
            (Class.Priest, 21, false, "디나르콜리", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카2
            (Class.Priest, 21, false, "디소루마", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카2
            (Class.Priest, 21, false, "에나르마", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카2
            (Class.Priest, 21, false, "쿠라노", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카2
            (Class.Priest, 41, false, "이모탈", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카2
            (Class.Priest, 50, false, "안티매직", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카2
            (Class.Priest, 55, false, "쿠라노소", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카3
            (Class.Priest, 63, false, "쿠라누스", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카3
            (Class.Priest, 73, false, "홀리랜서", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카3
            (Class.Priest, 81, false, "일루메나", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카4
            (Class.Priest, 83, false, "수페라쿠라노", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카4
            (Class.Priest, 87, false, "쿠라네라", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카4
            (Class.Priest, 93, false, "코마디아", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카4
            (Class.Priest, 99, false, "리베라토", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카4
            (Class.Priest, 99, false, "엑스쿠라네라", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카4
            (Class.Priest, 99, false, "엑스쿠라노", System.Array.Empty<string>(), System.Array.Empty<string>()), // 소라카4
            (Class.Monk, 11, false, "주먹단련", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신
            (Class.Monk, 11, false, "쿠로토", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신
            (Class.Monk, 15, true, "이형환위", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신
            (Class.Monk, 21, true, "양의신권", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신2
            (Class.Monk, 31, true, "단각", new string[] { "연천단각" }, System.Array.Empty<string>()), // 리신2
            (Class.Monk, 31, false, "장풍", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신2
            (Class.Monk, 41, false, "금강불괴", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신2
            (Class.Monk, 50, true, "구양신공", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신2
            (Class.Monk, 55, false, "쿠라노토", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신3
            (Class.Monk, 59, true, "붕각", new string[] { "붕신선각" }, System.Array.Empty<string>()), // 리신3
            (Class.Monk, 63, true, "선풍각", new string[] { "파천각" }, System.Array.Empty<string>()), // 리신3
            (Class.Monk, 70, true, "소수신공", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신3
            (Class.Monk, 74, true, "연환포", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신3
            (Class.Monk, 81, true, "달마신공", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신4
            (Class.Monk, 81, false, "일루메나", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신4
            (Class.Monk, 83, true, "일음지", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신4
            (Class.Monk, 87, true, "백보신권", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신4
            (Class.Monk, 93, true, "발경", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신4
            (Class.Monk, 99, false, "다라밀공", System.Array.Empty<string>(), System.Array.Empty<string>()), // 리신4
        };
    }
}
