# 치워 둔 Hades 영문 아이템 템플릿 (2026-09-30)

서버는 `templates/items/` 맨 위 파일만 읽으므로 여기 것은 게임에 실리지 않는다.
어디서도 쓰이지 않던 Hades 영문판(Group 하데스표) 932개다(드랍·상점·NPC·서버 코드·클라우드 캐릭터 37명 가방 모두 0).
같은 물건의 한글판(5.99 팩 → 원작 도감 값)이 게임에서 쓰는 쪽이다.
items 에 남긴 영문 아홉: 코드·자료가 이름으로 부르는 Apple · Gramail Prayer Necklace · Shirt · Blouse · Spider's Eye(거미 드랍),
Hades 기본 판 셋(Luathas Bronze Shield · Shagreen Boots · Luathas Coral Earrings — 시험이 쓴다), 5.99 무기 K4.
되돌리기: 파일을 `templates/items/` 로 다시 옮기면 된다.
접미사 장비 생성기(`scripts/build-suffix-gear-from-sheet.py`)는 그림 원본으로 이 폴더도 읽는다.
