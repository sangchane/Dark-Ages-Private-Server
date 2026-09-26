#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    [Script("Class Chooser")]
    public class ClassChooser : MundaneScript
    {
        public ClassChooser(GameServer server, Mundane mundane)
            : base(server, mundane)
        {
        }

        public override void OnClick(GameServer server, GameClient client)
        {
            if (client.Aisling.Path == Class.Peasant)
            {
                var options = new List<OptionsDataItem>
                {
                    new OptionsDataItem(0x06, "직업을 고르겠습니다."),
                    new OptionsDataItem(0x07, "아직 아닙니다.")
                };
                client.SendOptionsDialog(Mundane,
                    "음? 약해 보이는군. 아직 아무 직업도 없는 몸이야. 기술과 수련 없이는 이 세상에서 살아남을 수 없네. 이제 길을 골라야 할 때일세.",
                    options.ToArray());
            }
            else
            {
                client.SendOptionsDialog(Mundane, "이미 길을 골랐군.");
            }
        }

        public override void OnGossip(GameServer server, GameClient client, string message)
        {
        }

        public override void OnResponse(GameServer server, GameClient client, ushort responseID, string args)
        {
            if (responseID < 0x0001 ||
                responseID > 0x0005)
            {
                if (responseID == 6)
                {
                    var options = new List<OptionsDataItem>
                    {
                        new OptionsDataItem(0x01, "전사"),
                        new OptionsDataItem(0x02, "도적"),
                        new OptionsDataItem(0x03, "마법사"),
                        new OptionsDataItem(0x04, "성직자"),
                        new OptionsDataItem(0x05, "무도가")
                    };

                    client.SendOptionsDialog(Mundane, "어느 직업을 선택 하겠습니까?", options.ToArray());
                }

                if (responseID == 7)
                {
                }
            }
            else
            {
                client.Aisling.Path = (Class)responseID;

                client.SendOptionsDialog(Mundane, $"축하하네! 이제 자네는 {PathName(client.Aisling.Path)}일세.");

                client.Aisling.Stage = ClassStage.Master;
                client.Aisling.ExpLevel = 1;
                client.Aisling.StatPoints = 98 * 2;

                if (client.Aisling.Path == Class.Priest)
                {
                    Spell.GiveTo(client.Aisling, "armachd", 1);
                   
                    Spell.GiveTo(client.Aisling, "beag ioc", 1);
                    Spell.GiveTo(client.Aisling, "ao beag cradh", 1);

                    Spell.GiveTo(client.Aisling, "beag cradh", 1);
                  
                    Spell.GiveTo(client.Aisling, "deo saighead", 1);
              
                    Spell.GiveTo(client.Aisling, "pramh", 1);

                }

                if (client.Aisling.Path == Class.Wizard)
                {
                    
                    Spell.GiveTo(client.Aisling, "beag puinsein", 1);
                    
                 
                    Spell.GiveTo(client.Aisling, "pramh", 1);
                
                    Spell.GiveTo(client.Aisling, "sal", 1);
                    Spell.GiveTo(client.Aisling, "srad", 1);
                    Spell.GiveTo(client.Aisling, "athar", 1);

                   
               
                    Spell.GiveTo(client.Aisling, "fas nadur", 1);
                    
                 
                    var item = Item.Create(client.Aisling, ServerContext.GlobalItemTemplateCache["Magus Kronos"]);
                    

                    {
                        item.GiveTo(client.Aisling);
                       
                    }
                    var itemMJR = Item.Create(client.Aisling, ServerContext.GlobalItemTemplateCache["Magic Jade Ring"]);
                    {
                        itemMJR.GiveTo(client.Aisling);
                    }
                    var item3 = Item.Create(client.Aisling, ServerContext.GlobalItemTemplateCache["Used Boots"]);
                    {
                        item3.GiveTo(client.Aisling);
                    }
                }

                if (client.Aisling.Path == Class.Warrior)
                {
                    Skill.GiveTo(client.Aisling, "Wind Blade", 1);
                    Skill.GiveTo(client.Aisling, "Charge", 1);
                    Skill.GiveTo(client.Aisling, "Clobber", 1);
                    Skill.GiveTo(client.Aisling, "Assault", 1);
                    Skill.GiveTo(client.Aisling, "Wallop", 1);
                    Skill.GiveTo(client.Aisling, "Two-Handed Attack", 1);
                    Skill.GiveTo(client.Aisling, "Titan's Cleave", 1);
                    Skill.GiveTo(client.Aisling, "Rush", 1);
                    Skill.GiveTo(client.Aisling, "Rescue", 1);
                    Skill.GiveTo(client.Aisling, "beag suain ia gar", 1);
                    Skill.GiveTo(client.Aisling, "beag suain", 1);
                  
                    var item = Item.Create(client.Aisling, ServerContext.GlobalItemTemplateCache["Wooden Shield 2"]);
                    {
                        item.GiveTo(client.Aisling);
                    }
                    var item3 = Item.Create(client.Aisling, ServerContext.GlobalItemTemplateCache["Used Boots"]);
                    {
                        item3.GiveTo(client.Aisling);
                    }
                    var MaleChest = Item.Create(client.Aisling, ServerContext.GlobalItemTemplateCache["Torn Leather Tunic"]);
                    {
                        MaleChest.GiveTo(client.Aisling);
                    }
                    var Sword = Item.Create(client.Aisling, ServerContext.GlobalItemTemplateCache["Eppe"]);
                    {
                        Sword.GiveTo(client.Aisling);
                    }
                }

                if (client.Aisling.Path == Class.Monk)
                {
                    // 새 무도가는 이형환위·단각·쿠로토로 시작한다(사용자 2026-09-27).
                    // 기본 공격 Assail은 공격 단추(0x13)에 필요하므로 그대로 둔다.
                    Skill.GiveTo(client.Aisling, "이형환위", 1);
                    Skill.GiveTo(client.Aisling, "단각", 1);
                    Spell.GiveTo(client.Aisling, "쿠로토", 1);
                }

                if (client.Aisling.Path == Class.Rogue)
                {
                    Skill.GiveTo(client.Aisling, "Unstuck", 1);
                    Skill.GiveTo(client.Aisling, "Locate Player", 1);
                    Skill.GiveTo(client.Aisling, "Locate Monster", 1);
                    Skill.GiveTo(client.Aisling, "Sneak", 1);
                    Skill.GiveTo(client.Aisling, "Rescue", 1);
                    Skill.GiveTo(client.Aisling, "Stab", 1);
                    Skill.GiveTo(client.Aisling, "Inspect Item", 1);

                    Spell.GiveTo(client.Aisling, "beag ioc fein", 1);
                    Spell.GiveTo(client.Aisling, "Poison Trap", 1);
                    Spell.GiveTo(client.Aisling, "Needle Trap", 1);
                    Spell.GiveTo(client.Aisling, "Stiletto Trap", 1);

                    var item = Item.Create(client.Aisling, ServerContext.GlobalItemTemplateCache["Snow Secret"]);
                    {
                        item.GiveTo(client.Aisling);
                    }
                }

                client.CloseDialog();

                client.Aisling.LegendBook.AddLegend(new Legend.LegendItem
                {
                    Category = "Class",
                    Color = (byte)LegendColor.Blue,
                    Icon = (byte)LegendIcon.Victory,
                    Value = $"{PathName(client.Aisling.Path)}의 길에 들어섬"
                });

                client.Aisling.LegendBook.AddLegend(new Legend.LegendItem
                {
                    Category = "Alpha Aisling",
                    Color = (byte)LegendColor.Yellow,
                    Icon = (byte)LegendIcon.Heart,
                    Value = $"첫 아이슬링 - 처음의 혹독한 겨울을 견뎌 냄"
                });

                client.Aisling.GoHome();
                client.SendStats(StatusFlags.All);

                Task.Delay(350).ContinueWith(ct => { client.Aisling.Animate(5); });
            }
        }

        private static string PathName(Class path) => path switch
        {
            Class.Warrior => "전사",
            Class.Rogue => "도적",
            Class.Wizard => "마법사",
            Class.Priest => "성직자",
            Class.Monk => "무도가",
            _ => path.ToString()
        };

        public override void TargetAcquired(Sprite Target)
        {
        }
    }
}
