#region

using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    /// <summary>
    /// 운영자 상인 — 어둠템에 없는 센 팩 장비(체·마 +1000 이상, 레벨 99 밑)를 운영자(<c>GameMasters</c>)에게만 판다(사용자 2026-10-09
    /// 「5번장비만 취급하는 npc 따로 만들어둬 운영자 권한으로만 들어갈 수 있거나 구매할 수 있게해서」, 물목은
    /// <c>scripts/gen/items/build-operator-shop.py</c>). 그 밖에는 <see cref="shop1" /> 과 같다. 앱의 일괄 거래도 같은 문을 지난다
    /// (<c>GameServerHandlers</c> 일괄 거래).
    /// </summary>
    [Script("operator_shop", "LOD")]
    public class operator_shop : shop1
    {
        public operator_shop(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        public override void OnClick(GameServer server, GameClient client)
        {
            if (client.Aisling.GameMaster)
                base.OnClick(server, client);
            else
                client.SendOptionsDialog(Mundane, "운영자만 거래할 수 있습니다.");
        }

        public override void OnResponse(GameServer server, GameClient client, ushort responseID, string args)
        {
            if (client.Aisling.GameMaster)
                base.OnResponse(server, client, responseID, args);
        }
    }
}
