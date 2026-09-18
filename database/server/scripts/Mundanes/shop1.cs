#region

using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    [Script("shop1", "Dean")]
    public class shop1 : MundaneScript
    {
        public shop1(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        public override void OnClick(GameServer server, GameClient client)
        {
            var opts = new List<OptionsDataItem>
            {
                new OptionsDataItem(0x0001, "삽니다"),
                new OptionsDataItem(0x0002, "팝니다"),
                new OptionsDataItem(0x0003, "수리합니다")
            };

            client.SendOptionsDialog(Mundane, "무엇을 찾으십니까?", opts.ToArray());
        }

        public override void OnGossip(GameServer server, GameClient client, string message)
        {
        }

        public override void OnResponse(GameServer server, GameClient client, ushort responseID, string args)
        {
            var defaultbag = Mundane.Template.DefaultMerchantStock.Select(i =>
                ServerContext.GlobalItemTemplateCache.ContainsKey(i)
                    ? ServerContext.GlobalItemTemplateCache[i]
                    : null);

            var opts = new List<OptionsDataItem>();
            switch (responseID)
            {
                case 0x0001:
                    client.SendItemShopDialog(Mundane, "천천히 둘러보십시오.", 0x0004,
                        ServerContext.GlobalItemTemplateCache.Values.Where(i => i.NpcKey == Mundane.Template.Name)
                            .ToList().Concat(defaultbag.Where(n => n != null)));
                    break;

                case 0x0002:
                    client.SendItemSellDialog(Mundane, "무엇을 파시겠습니까?", 0x0005,
                        client.Aisling.Inventory.Items.Values.Where(i => i != null && i.Template != null)
                            .Select(i => i.Slot).ToList());

                    break;

                case 0x0500:
                {
                    var item = client.Aisling.Inventory.Get(i => i != null && i.Slot == Convert.ToInt32(args))
                        .FirstOrDefault();
                    var offer = Convert.ToString((int) (item.Template.Value / 1.6));

                    var opts2 = new List<OptionsDataItem>
                    {
                        new OptionsDataItem(0x0019, "그렇게 하지요"), new OptionsDataItem(0x0020, "그만두겠습니다")
                    };

                    client.SendOptionsDialog(Mundane,
                        $"{item.Template.Name}, {offer} 전에 사겠습니다. 괜찮으십니까?", item.Template.Name,
                        opts2.ToArray());
                }
                    break;

                case 0x0019:
                {
                    var v = args;
                    var item = client.Aisling.Inventory.Get(i => i != null && i.Template.Name == v)
                        .FirstOrDefault();

                    if (item == null)
                        return;

                    var offer = Convert.ToString((int) (item.Template.Value / 1.6));

                    if (Convert.ToInt32(offer) <= 0)
                        return;

                    if (Convert.ToInt32(offer) > item.Template.Value)
                        return;

                    if (client.Aisling.GoldPoints + Convert.ToInt32(offer) <=
                        ServerContext.Config.MaxCarryGold)
                    {
                        client.Aisling.GoldPoints += Convert.ToInt32(offer);
                        client.Aisling.EquipmentManager.RemoveFromInventory(item, true);
                        client.SendStats(StatusFlags.StructC);

                        client.SendOptionsDialog(Mundane, "잘 샀습니다. 또 오십시오.");
                    }
                }
                    break;

                #region Buy

                case 0x0003:

                    var repair_sum = client.Aisling.Inventory.Items.Where(i => i.Value != null
                                                                               && i.Value.Template.Flags.HasFlag(
                                                                                   ItemFlags.Repairable)).Sum(i =>
                        i.Value.Template.Value / 4);

                    opts.Add(new OptionsDataItem(0x0014, "그렇게 하지요"));
                    opts.Add(new OptionsDataItem(0x0015, "그만두겠습니다"));
                    client.SendOptionsDialog(Mundane,
                        "모두 손보는 데 " + repair_sum + " 전이 듭니다. 맡기시겠습니까?",
                        repair_sum.ToString(), opts.ToArray());

                    break;

                case 0x0014:
                {
                    var gear = client.Aisling.EquipmentManager.Equipment.Where(i => i.Value != null)
                        .Select(i => i.Value.Item);

                    client.RepairEquipment(gear);

                    client.SendOptionsDialog(Mundane, "다 손봤습니다. 살펴 가십시오.");
                }
                    break;

                case 0x0015:
                    client.SendOptionsDialog(Mundane, "그러시지요. 다음에 뵙겠습니다.");
                    break;

                case 0x0004:
                {
                    if (string.IsNullOrEmpty(args))
                        return;

                    if (!ServerContext.GlobalItemTemplateCache.ContainsKey(args))
                        return;

                    var template = ServerContext.GlobalItemTemplateCache[args];
                    if (template != null)
                        if (client.Aisling.GoldPoints >= template.Value)
                        {
                            var item = Item.Create(client.Aisling, template);

                            if (item.GiveTo(client.Aisling))
                            {
                                client.Aisling.GoldPoints -= (int) template.Value;

                                if (client.Aisling.GoldPoints < 0)
                                    client.Aisling.GoldPoints = 0;

                                client.SendStats(StatusFlags.All);
                                client.SendOptionsDialog(Mundane, $"{args}, 여기 있습니다.");
                            }
                            else
                            {
                                client.SendMessage(0x02, "더 들 수 없습니다.");
                            }
                        }
                        else
                        {
                            // 돈이 모자란다고 손님에게 저주를 걸던 자리다(beag cradh). 원작 상점은 그러지 않는다.
                            client.SendOptionsDialog(Mundane, "돈이 모자랍니다.");
                        }
                }
                    break;

                #endregion Buy
            }
        }

        public override void TargetAcquired(Sprite Target)
        {
        }
    }
}