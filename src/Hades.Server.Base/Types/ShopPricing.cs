namespace Darkages.Types
{
    /// <summary>상점이 손님 물건을 사 주는 값 — 앱 일괄 판매(0xF2)와 상점 스크립트(shop1·shop2)가 같은 식을 쓴다.</summary>
    public static class ShopPricing
    {
        /// <summary>물건 값의 1/1.6(약 62%), 소수점 아래 버림 — 원본 Hades 상점 스크립트의 식 그대로.</summary>
        public static int Offer(Item item) => (int) (item.Template.Value / 1.6);
    }
}
