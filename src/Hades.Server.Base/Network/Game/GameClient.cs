#region

using Darkages.Common;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Storage;
using Darkages.Storage.locales.debuffs;
using Darkages.Types;
using MenuInterpreter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Darkages.Templates;
using Newtonsoft.Json;


#endregion

namespace Darkages.Network.Game
{
    public partial class GameClient : NetworkClient
    {
        public bool MapUpdating;
        private readonly object _syncObj = new object();

        public GameClient()
        {
            HpRegenTimer = new GameServerTimer(
                TimeSpan.FromMilliseconds(ServerContext.Config.RegenRate));

            MpRegenTimer = new GameServerTimer(
                TimeSpan.FromMilliseconds(ServerContext.Config.RegenRate));
        }

        public Aisling Aisling { get; set; }

        public DateTime BoardOpened { get; set; }

        public bool CanSendLocation =>
            DateTime.UtcNow - LastLocationSent < new TimeSpan(0, 0, 0, 2);

        public DialogSession DlgSession { get; set; }

        public GameServerTimer HpRegenTimer { get; set; }

        public bool IsRefreshing =>
            DateTime.UtcNow - LastClientRefresh < new TimeSpan(0, 0, 0, 0, ServerContext.Config.RefreshRate);

        public bool IsMoving =>
            DateTime.UtcNow - LastMovement > new TimeSpan(0, 0, 0, 0, 850);

        public bool IsSpeedHacking => 
            DateTime.UtcNow - LastMovement < TimeSpan.FromMilliseconds(ServerContext.Config.WalkingSpeedLimitFactor);

        public bool IsWarping =>
            DateTime.UtcNow - LastWarp < new TimeSpan(0, 0, 0, 0, ServerContext.Config.WarpCheckRate);

        public Stack<CastInfo> CastStack = new Stack<CastInfo>();

        public byte LastActivatedSlot { get; set; }
        public DateTime LastAssail { get; set; }
        public ushort LastBoardActivated { get; set; }
        public DateTime LastClientRefresh { get; set; }
        public Item LastItemDropped { get; set; }
        public DateTime LastLocationSent { get; set; }
        public DateTime LastMapUpdated { get; set; }
        public TimeSpan LastMenuStarted { get; set; }
        public DateTime LastMessageSent { get; set; }
        public DateTime LastMovement { get; set; }
        public DateTime LastPing { get; set; }
        public DateTime LastPingResponse { get; set; }
        public DateTime LastSave { get; set; }
        public DateTime LastScriptExecuted { get; set; }
        public DateTime LastWarp { get; set; }
        public DateTime LastWhisperMessageSent { get; set; }
        public Interpreter MenuInterpter { get; set; }
        public GameServerTimer MpRegenTimer { get; set; }
        [JsonIgnore] public PendingSell PendingItemSessions { get; set; }
        public GameServer Server { get; set; }
        public bool ShouldUpdateMap { get; set; }

        [JsonIgnore] public DateTime LastNodeClicked { get; set; }
        [JsonIgnore] public WorldPortal PendingNode { get; set; }

        public bool WasUpdatingMapRecently =>
            DateTime.UtcNow - LastMapUpdated < new TimeSpan(0, 0, 0, 0, 100);

        public Position LastKnownPosition { get; set; }


        [JsonIgnore]
        public int MapClicks { get; set; }


        public GameClient AislingToGhostForm()
        {
            Aisling.Flags = AislingFlags.Ghost;

            HpRegenTimer.Disabled = true;
            MpRegenTimer.Disabled = true;

            Refresh(true);

            return this;
        }

        public void BuildSettings()
        {
            if (ServerContext.Config.Settings == null || ServerContext.Config.Settings.Count == 0)
                return;

            if (Aisling.GameSettings == null || Aisling.GameSettings.Count == 0)
            {
                Aisling.GameSettings = new List<ClientGameSettings>();

                foreach (var settings in ServerContext.Config.Settings)
                    Aisling.GameSettings.Add(new ClientGameSettings(settings.SettingOff, settings.SettingOn,
                        settings.Enabled));
            }
        }

        public bool CheckReqs(GameClient client, Item item)
        {
            var message = string.Empty;

            if (client.Aisling.GameMaster || client.Aisling.Developer)
            {
                if (item.Durability > 1)
                {
                    client.Aisling.EquipmentManager.Add(item.Template.EquipmentSlot, item);
                    return true;
                }
            }

            if (client.Aisling.ExpLevel < item.Template.LevelRequired)
            {
                message = ServerContext.Config.CantWearYetMessage;
                if (!(message != null && string.IsNullOrEmpty(message)))
                {
                    client.SendMessage(0x02, message);
                    return false;
                }
            }

            if (item.Durability <= 0)
            {
                message = ServerContext.Config.RepairItemMessage;
                if (!(message != null && string.IsNullOrEmpty(message)))
                {
                    client.SendMessage(0x02, message);
                    return false;
                }
            }

            if (client.Aisling.Path != item.Template.Class && item.Template.Class != Class.Peasant)
                message = client.Aisling.ExpLevel >= item.Template.LevelRequired
                    ? ServerContext.Config.WrongClassMessage
                    : ServerContext.Config.CantWearYetMessage;

            if (!item.Template.Class.HasFlag(client.Aisling.Path) && item.Template.Class != Class.Peasant)
                message = "감히 사용할 수 없습니다.";

            if (!(message != null && string.IsNullOrEmpty(message)))
            {
                client.SendMessage(0x02, message);
                return false;
            }

            if (client.Aisling.ExpLevel >= item.Template.LevelRequired
                && (client.Aisling.Path == item.Template.Class || item.Template.Class == Class.Peasant))
            {
                if (item.Template.Gender == Gender.Both)
                {
                    client.Aisling.EquipmentManager.Add(item.Template.EquipmentSlot, item);
                }
                else
                {
                    if (item.Template.Gender == client.Aisling.Gender)
                    {
                        client.Aisling.EquipmentManager.Add(item.Template.EquipmentSlot, item);
                    }
                    else
                    {
                        client.SendMessage(0x02, ServerContext.Config.DoesNotFitMessage);
                        return false;
                    }
                }

                return true;
            }

            client.SendMessage(0x02, ServerContext.Config.CantEquipThatMessage);
            return false;
        }

        public GameClient CloseDialog()
        {
            Send(new byte[] {0x30, 0x00, 0x0A, 0x00});
            MenuInterpter = null;

            return this;
        }

        public GameClient DoUpdate(TimeSpan elapsedTime)
        {
            ObjectCheckPoint();

            DispatchCasts();

            return HandleTimeOuts()
                .StatusCheck()
                .Regen(elapsedTime)
                .UpdateStatusBar(elapsedTime)
                .UpdateReactors(elapsedTime);
        }

        private void ObjectCheckPoint()
        {
            // This runs on the update loop for every client. A client whose character is not in hand yet —
            // signing in, or already gone — used to throw here once per aisling in the world, which under
            // ten connections meant tens of thousands of exceptions a minute and hid everything else.
            var self = Aisling;

            if (self == null)
            {
                return;
            }

            var clones = GetObjects<Aisling>(null,
                p => p != null &&
                     string.Equals(p.Username, self.Username, StringComparison.CurrentCultureIgnoreCase) &&
                     p.Serial != self.Serial).ToArray();

            if (clones.Length <= 0) return;
            foreach (var aisling in clones)
            {
                if (Aisling != null && aisling != null)
                {
                    aisling.HideFrom(Aisling);
                    Aisling.HideFrom(aisling);
                }

                if (aisling?.Client != null)
                {
                    aisling.Remove(true);
                    Server.ClientDisconnected(aisling.Client);
                }
            }

            DelObjects(clones);
        }

        public GameClient EnterArea()
        {
            return Enter();
        }

        public GameClient HandleTimeOuts()
        {
            if (Aisling.Exchange?.Trader == null)
                return this;

            if (!Aisling.Exchange.Trader.LoggedIn
                || !Aisling.WithinRangeOf(Aisling.Exchange.Trader))
                Aisling.CancelExchange();

            return this;
        }

        public GameClient InitSpellBar()
        {
            lock (_syncObj)
            {
                foreach (var buff in Aisling.Buffs.Select(i => i.Value))
                {
                    buff.OnApplied(Aisling, buff);
                    {
                        buff.Display(Aisling);
                    }
                }

                foreach (var debuff in Aisling.Debuffs.Select(i => i.Value))
                {
                    debuff.OnApplied(Aisling, debuff);
                    {
                        debuff.Display(Aisling);
                    }
                }
            }

            return this;
        }

        public GameClient Insert()
        {
            var self = Aisling;

            if (self == null)
            {
                return this;
            }

            // The predicate walks every aisling in the world. One of them being half built or already gone
            // threw, and the throw came out of the middle of entering the world — the character was never
            // placed and the player was left with a blank screen. Ten connections signing in and out hit
            // this about a third of the time.
            var obj = GetObject<Aisling>(null,
                aisling => aisling != null &&
                           (aisling.Serial == self.Serial ||
                            string.Equals(aisling.Username, self.Username,
                                StringComparison.CurrentCultureIgnoreCase)));

            if (obj == null)
            {
                AddObject(self);
            }
            else
            {
                obj.Remove();
                AddObject(self);
            }

            return this;
        }

        public void Interupt()
        {
            GameServer.CancelIfCasting(this);
            SendLocation();
        }


        public GameClient LeaveArea(bool update = false, bool delete = false)
        {
            if (Aisling.LastMapId == short.MaxValue) Aisling.LastMapId = Aisling.CurrentMapId;

            Aisling.Remove(update, delete);

            if (ServerContext.Config.F5ReloadsPlayers)
                foreach (var obj in Aisling.AislingsNearby())
                {
                    if (obj.Serial == Aisling.Serial)
                        continue;

                    obj.HideFrom(Aisling);
                    obj.ShowTo(Aisling);
                }

            if (ServerContext.Config.F5ReloadsMonsters)
                foreach (var obj in Aisling.MonstersNearby())
                {
                    if (obj.Serial == Aisling.Serial)
                        continue;

                    obj.HideFrom(Aisling);
                    obj.ShowTo(Aisling);
                }

            return this;
        }

        public GameClient Load()
        {
            if (Aisling == null)
                return null;

            if (!ServerContext.GlobalMapCache.ContainsKey(Aisling.AreaId))
            {
                // 없어진 맵(서버가 다시 켜지며 사라진 개인 사본 따위)에 저장된 캐릭터는 들어오지 못하던 채로 남았다 — 시작 자리로 보낸다.
                if (!ServerContext.GlobalMapCache.ContainsKey(ServerContext.Config.StartingMap))
                    return null;

                Aisling.CurrentMapId = ServerContext.Config.StartingMap;
                Aisling.XPos = ServerContext.Config.StartingPosition.X;
                Aisling.YPos = ServerContext.Config.StartingPosition.Y;
            }

            SetAislingStartupVariables();

            lock (_syncObj)
            {
                try
                {
                    return InitSpellBar()
                        .LoadInventory()
                        .LoadSkillBook()
                        .LoadSpellBook()
                        .LoadEquipment()
                        .SendProfileUpdate()
                        .SendStats(StatusFlags.All);
                }
                catch (NullReferenceException ex)
                {
                    ServerContext.Logger(ex.Message, Microsoft.Extensions.Logging.LogLevel.Error);
                    ServerContext.Logger(ex.StackTrace, Microsoft.Extensions.Logging.LogLevel.Error);
                }
            }


            return null;
        }

        public GameClient LoadEquipment()
        {
            var formats = new List<NetworkFormat>();

            lock (_syncObj)
            {
                foreach (var item in Aisling.EquipmentManager.Equipment)
                {
                    var equipment = item.Value;

                    if (equipment?.Item?.Template == null)
                        continue;

                    if (equipment.Item.Template != null)
                        if (ServerContext.GlobalItemTemplateCache.ContainsKey(equipment.Item.Template.Name))
                        {
                            var template = ServerContext.GlobalItemTemplateCache[equipment.Item.Template.Name];
                            {
                                item.Value.Item.Template = template;

                                if (item.Value.Item.Upgrades > 0)
                                    Item.ApplyQuality(item.Value.Item);
                            }
                        }

                    equipment.Item.Scripts =
                        ScriptManager.Load<ItemScript>(equipment.Item.Template.ScriptName, equipment.Item);
                    if (!string.IsNullOrEmpty(equipment.Item.Template.WeaponScript))
                        equipment.Item.WeaponScripts =
                            ScriptManager.Load<WeaponScript>(equipment.Item.Template.WeaponScript, equipment.Item);

                    if (equipment.Item.Scripts?.Values != null)
                        foreach (var script in equipment.Item.Scripts?.Values)
                            script.Equipped(Aisling, (byte) equipment.Slot);

                    if (equipment.Item.CanCarry(Aisling))
                    {
                        Aisling.CurrentWeight += equipment.Item.Template.CarryWeight;

                        formats.Add(new ServerFormat37(equipment.Item, (byte) equipment.Slot));
                    }
                    else
                    {
                        var nitem = Clone<Item>(item.Value.Item);
                        nitem.Release(Aisling, Aisling.Position);

                        SendMessage(0x02,
                            string.Format(CultureInfo.CurrentCulture, "{0}: 너무 무거워서 들 수 없습니다.",
                                nitem.Template.Name));

                        continue;
                    }

                    if ((equipment.Item.Template.Flags & ItemFlags.Equipable) == ItemFlags.Equipable)
                        for (var i = 0; i < Aisling.SpellBook.Spells.Count; i++)
                        {
                            var spell = Aisling.SpellBook.FindInSlot(i);
                            if (spell?.Template != null)
                                equipment.Item.UpdateSpellSlot(this, spell.Slot);
                        }
                }
            }

            foreach (var format in formats)
                Aisling.Client.Send(format);

            return this;
        }

        public GameClient LoadInventory()
        {
            lock (_syncObj)
            {
                var itemsAvailable = Aisling.Inventory.Items.Values
                    .Where(i => i?.Template != null).ToArray();

                foreach (var item in itemsAvailable)
                {
                    if (string.IsNullOrEmpty(item.Template.Name))
                        continue;

                    item.Scripts = ScriptManager.Load<ItemScript>(item.Template.ScriptName, item);

                    if (!string.IsNullOrEmpty(item.Template.WeaponScript))
                        item.WeaponScripts = ScriptManager.Load<WeaponScript>(item.Template.WeaponScript, item);

                    if (ServerContext.GlobalItemTemplateCache.ContainsKey(item.Template.Name))
                    {
                        var template = ServerContext.GlobalItemTemplateCache[item.Template.Name];
                        {
                            item.Template = template;
                        }

                        if (Aisling.GameMaster) item.Upgrades = 10;

                        if (item.Upgrades > 0)
                            Item.ApplyQuality(item);
                    }

                    if (item.Template != null)
                    {
                        if (Aisling.CurrentWeight + item.Template.CarryWeight < Aisling.MaximumWeight)
                        {
                            var format = new ServerFormat0F(item);
                            Send(format);

                            Aisling.Inventory.Set(item, false);
                            Aisling.CurrentWeight += item.Template.CarryWeight;
                        }
                        else
                        {
                            var copy = Clone<Item>(item);
                            {
                                copy.Release(Aisling, Aisling.Position);

                                SendMessage(0x02,
                                    string.Format(CultureInfo.CurrentCulture, "비틀거리다 {0}을(를) 떨어뜨렸습니다.",
                                        item.Template.Name));
                            }
                        }
                    }
                }
            }

            return this;
        }

        public GameClient LoadSkillBook()
        {
            lock (_syncObj)
            {
                var skillsAvailable = Aisling.SkillBook.Skills.Values
                    .Where(i => i?.Template != null).ToArray();

                foreach (var skill in skillsAvailable)
                {
                    if (skill.Template != null)
                    {
                        var skillName = skill.Template.Name;

                        if (ServerContext.GlobalSkillTemplateCache.ContainsKey(skillName))
                        {
                            var template = ServerContext.GlobalSkillTemplateCache[skillName];
                            {
                                skill.Template = template;
                            }
                        }
                    }

                    skill.InUse = false;
                    skill.NextAvailableUse = DateTime.UtcNow;

                    Send(new ServerFormat2C(skill.Slot,
                        skill.Icon,
                        skill.Name));

                    if (skill.Template != null)
                        skill.Scripts = ScriptManager.Load<SkillScript>(skill.Template.ScriptName, skill);

                    Aisling.SkillBook.Set(skill, false);
                }
            }

            return this;
        }

        public GameClient LoadSpellBook()
        {
            lock (_syncObj)
            {
                Lorule.Update(() =>
                {
                    var spellsAvailable = Aisling.SpellBook.Spells.Values
                        .Where(i => i?.Template != null).ToArray();

                    foreach (var spell in spellsAvailable)
                    {
                        if (spell.Template != null)
                            if (ServerContext.GlobalSpellTemplateCache.ContainsKey(spell.Template.Name))
                            {
                                var template = ServerContext.GlobalSpellTemplateCache[spell.Template.Name];
                                {
                                    spell.Template = template;
                                }
                            }

                        if (spell.Template != null)
                        {
                            spell.Lines = spell.Template.BaseLines;

                            Spell.AttachScript(spell);
                            {
                                Aisling.SpellBook.Set(spell, false);
                            }

                            Send(new ServerFormat17(spell));

                            if (spell.NextAvailableUse.Year > 1)
                            {
                                var spell1 = spell;

                                Task.Delay(1000).ContinueWith(ct =>
                                {
                                    var delta = (int) Math.Abs((DateTime.UtcNow - spell1.NextAvailableUse)
                                        .TotalSeconds);

                                    if (delta <= spell1.Template.Cooldown)
                                        Send(new ServerFormat3F(0,
                                            spell1.Slot,
                                            delta));
                                });
                            }
                            else
                            {
                                spell.NextAvailableUse = DateTime.UtcNow;
                            }
                        }
                    }
                });
            }

            return this;
        }

        public GameClient PayItemPrerequisites(LearningPredicate prerequisites)
        {
            if (prerequisites.Items_Required != null && prerequisites.Items_Required.Count > 0)
                foreach (var retainer in prerequisites.Items_Required)
                {
                    var item = Aisling.Inventory.Get(i => i.Template.Name == retainer.Item);

                    foreach (var i in item)
                        if (!i.Template.Flags.HasFlag(ItemFlags.Stackable))
                        {
                            Aisling.EquipmentManager.RemoveFromInventory(i, i.Template.CarryWeight > 0);
                            break;
                        }
                        else
                        {
                            Aisling.Inventory.RemoveRange(Aisling.Client, i, retainer.AmountRequired);
                            break;
                        }
                }

            return this;
        }

        public bool PayPrerequisites(LearningPredicate prerequisites)
        {
            if (prerequisites == null) return false;

            PayItemPrerequisites(prerequisites);
            {
                if (prerequisites.Gold_Required > 0)
                {
                    Aisling.GoldPoints -= prerequisites.Gold_Required;
                    if (Aisling.GoldPoints <= 0)
                        Aisling.GoldPoints = 0;
                }

                SendStats(StatusFlags.All);
                return true;
            }
        }

        /// <summary>
        /// 갇혔으면 꺼낸다. 벽 속에 서 있거나 네 칸이 모두 막혀 있으면, 가장 가까운 트인 칸으로 옮긴다 —
        /// 그런 자리에서는 방향키를 아무리 눌러도 아무 일도 일어나지 않아, 사람 눈에는 게임이 멎은 것과 같다
        /// (사용자, 2026-09-18). 어디서 그렇게 됐는지 몰라도 스스로 빠져나온다.
        /// </summary>
        public bool FreeIfStuck()
        {
            if (Aisling?.Map == null || !Aisling.Map.Enclosed(Aisling.X, Aisling.Y))
                return false;

            if (Aisling.Map.FreeWayOut(Aisling.X, Aisling.Y) is not { } out_)
                return false;

            Aisling.XPos = out_.X;
            Aisling.YPos = out_.Y;
            SendLocation();
            SystemMessage("길이 막혀 가까운 곳으로 옮겼습니다.");

            return true;
        }

        public GameClient Refresh(bool delete = false)
        {
            LeaveArea(delete);
            EnterArea();

            return this;
        }

        public GameClient RefreshMap(bool updateView = false)
        { 
            // 시야를 비우기 **전에** 보여 주던 것을 거둔다. 비우기만 하면 서버는 그것들을 다시는 거두지 않는다 —
            // ObjectComponent 는 시야에 든 것만 0x0E 로 지운다. 0x15 에 화면을 비우지 않는 클라이언트(모바일)에는
            // 사냥터 괴물이 움직이지도 죽지도 않는 채로 마을 위에 남았고, 마을이 더 작으면 맵 밖에 섰다
            // (2026-09-24 사용자 보고 "마을에 좀비 괴물"). 같은 맵 순간이동·이형환위도 이 길을 탄다.
            Sprite[] shown;

            try
            {
                shown = Aisling.View.ToArray();
            }
            catch (InvalidOperationException)
            {
                // 다른 스레드가 시야를 고치던 중이면 이번에는 거두지 못한다 — 서버 다른 곳처럼 넘어간다.
                shown = Array.Empty<Sprite>();
            }

            foreach (var seen in shown)
                if (seen != null && seen.Serial != Aisling.Serial)
                    seen.HideFrom(Aisling);

            Aisling.View.Clear();
            ShouldUpdateMap = false;

            if (Aisling.CurrentMapId != Aisling.LastMapId)
            {
                ShouldUpdateMap = true;
                Aisling.LastMapId = Aisling.CurrentMapId;

                if (Aisling.DiscoveredMaps.All(i => i != Aisling.CurrentMapId))
                    Aisling.DiscoveredMaps.Add(Aisling.CurrentMapId);

                SendMusic();

                if (Aisling.Map != null && Aisling.Map.Scripts.Any())
                {
                    foreach (var script in Aisling.Map.Scripts.Values)
                    {
                        script.OnMapExit(this);
                    }
                }
            }


            foreach (var obj in GetObjects<Monster>(null, i => i?.Summoner != null))
            {
                obj.ShowTo(Aisling);
            }

            //if (!ShouldUpdateMap)
            //    return this;

            MapUpdating = true;
            Aisling.Client.LastMapUpdated = DateTime.UtcNow;

            if (Aisling.Blind == 1)
                if (Aisling.Map != null && !Aisling.Map.Flags.HasFlag(MapFlags.Darkness))
                    Aisling.Map.Flags |= MapFlags.Darkness;

            Send(new ServerFormat15(Aisling.Map));

            if (Aisling.Map != null && Aisling.Map.Scripts == null && ShouldUpdateMap)
            {
                if (!string.IsNullOrEmpty(Aisling.Map.ScriptKey))
                {
                    Aisling.Map.Scripts = ScriptManager.Load<AreaScript>(Aisling.Map.ScriptKey, Aisling.Map);
                }
            }

            if (Aisling.Map?.Scripts != null && (Aisling.Map == null || !Aisling.Map.Scripts.Any()))
                return this;

            if (Aisling.Map?.Scripts == null) return this;
            {
                foreach (var script in Aisling.Map.Scripts.Values)
                {
                    script.OnMapEnter(this);
                }
            }

            return this;
        }

        /// <summary>
        /// 한 번에 저절로 차는 체력(마력). 5.99 의 식 그대로(Novaonline.exe 0x46d25a~0x46d35f):
        /// 최대 ÷ 100 × 능력치 ÷ 4.3 을 버림하고, 최대의 15% 아래면 15%, 25% 위거나 능력치가 108 이상이면 25%.
        /// 백분율은 최대 ÷ 100 을 먼저 버림한 뒤 곱한다. 5.99 가 한 가지 더 보는 캐릭터 칸(+98, 켜져 있으면 1.5배)은
        /// 무엇인지 몰라 옮기지 않았다.
        /// </summary>
        public static int NaturalRecovery(int maximum, int attribute)
        {
            var hundredth = (int) (maximum / 100.0);
            var amount = (int) (maximum / 100.0 * (attribute / 4.3));

            if (amount < hundredth * 15)
                amount = hundredth * 15;

            if (amount > hundredth * 25 || attribute >= 108)
                amount = hundredth * 25;

            return amount;
        }

        public GameClient Regen(TimeSpan elapsedTime)
        {
            if (Aisling.Con > Aisling.ExpLevel + 1)
                HpRegenTimer.Delay = TimeSpan.FromMilliseconds(ServerContext.Config.RegenRate + 1 / 2);

            if (Aisling.Wis > Aisling.ExpLevel + 1)
                MpRegenTimer.Delay = TimeSpan.FromMilliseconds(ServerContext.Config.RegenRate + 1 / 2);

            var a = false;
            var b = false;

            if (!HpRegenTimer.Disabled)
                a = HpRegenTimer.Update(elapsedTime);

            if (!MpRegenTimer.Disabled)
                b = MpRegenTimer.Update(elapsedTime);

            // 5.99 서버(Novaonline.exe 0x46d1a5)는 한 함수에서 체력은 지구력으로, 마력은 지혜로 채운다. 둘 다
            // 장비를 뺀 능력치(캐릭터 +163 · +162)이고, 장비의 회복 칸 합(+152)을 그대로 더한다 — 하데스에서 그
            // 자리는 Regen 이다. 주기는 21초(0x4766aa)라 LoruleConfig 의 RegenRate 가 정한다.
            // 전에는 5초마다 최대의 10~20% 를 채워서, 괴물 하나가 치는 것보다 빨리 차올랐다.
            if (a && !HpRegenTimer.Disabled)
                Aisling.CurrentHp = (Aisling.CurrentHp + NaturalRecovery(Aisling.MaximumHp, Aisling._Con) + Aisling.Regen)
                    .Clamp(0, Aisling.MaximumHp);

            if (b && !MpRegenTimer.Disabled)
                Aisling.CurrentMp = (Aisling.CurrentMp + NaturalRecovery(Aisling.MaximumMp, Aisling._Wis) + Aisling.Regen)
                    .Clamp(0, Aisling.MaximumMp);

            if (a || b) SendStats(StatusFlags.StructB);

            return this;
        }

        public void RepairEquipment(IEnumerable<Item> gear)
        {
            if (gear != null)
                foreach (var item in gear)
                {
                    if (item != null && item.Template.Flags.HasFlag(ItemFlags.Repairable))
                    {
                        item.Durability = item.Template.MaxDurability;
                    }
                }

            if (Aisling.Inventory.Items != null)
                foreach (var item in Aisling.Inventory.Items.Where(i => i.Value != null).Select(i => i.Value))
                {
                    item.Durability = item.Template.MaxDurability;
                    Aisling.Inventory.UpdateSlot(this, item);
                }
        }

        public bool Revive()
        {
            Aisling.Flags = AislingFlags.Normal;
            HpRegenTimer.Disabled = false;
            MpRegenTimer.Disabled = false;

            Aisling.Recover();
            return Aisling.CurrentHp > 0;
        }

        public GameClient Save()
        {
            if (Aisling != null)
            {
                StorageManager.AislingBucket.Save(Aisling);
                LastSave = DateTime.UtcNow;
                ServerContext.Logger($"Aisling {Aisling.Username} data has been saved.");
            }

            return this;
        }

        public void Say(string message, byte type = 0x00)
        {
            var response = new ServerFormat0D
            {
                Serial = Aisling.Serial,
                Type = type,
                Text = message
            };

            Aisling.Show(Scope.NearbyAislings, response);
        }

        public void SendAnimation(ushort animation, Sprite to, Sprite from, byte speed = 100)
        {
            var format = new ServerFormat29((uint) from.Serial, (uint) to.Serial, animation, 0, speed);
            Aisling.Show(Scope.NearbyAislings, format);
        }

        public void SendItemSellDialog(Mundane mundane, string text, ushort step, IEnumerable<byte> items)
        {
            Send(new ServerFormat2F(mundane, text, new ItemSellData(step, items)));
        }

        public void SendItemShopDialog(Mundane mundane, string text, ushort step, IEnumerable<ItemTemplate> items)
        {
            Send(new ServerFormat2F(mundane, text, new ItemShopData(step, items)));
        }

        public GameClient SendLocation()
        {
            Send(new ServerFormat04(Aisling));
            LastLocationSent = DateTime.UtcNow;
            return this;
        }

        public GameClient SendMessage(byte type, string text)
        {
            Send(new ServerFormat0A(type, text));
            LastMessageSent = DateTime.UtcNow;

            return this;
        }

        public GameClient SendMessage(string text)
        {
            Send(new ServerFormat0A(0x02, text));
            LastMessageSent = DateTime.UtcNow;

            return this;
        }

        public void SendMessage(Scope scope, byte type, string text)
        {
            switch (scope)
            {
                case Scope.Self:
                    SendMessage(type, text);
                    break;

                case Scope.NearbyAislings:
                {
                    var nearby = GetObjects<Aisling>(Aisling.Map, i => i.WithinRangeOf(Aisling));

                    foreach (var obj in nearby)
                        obj.Client.SendMessage(type, text);
                }
                    break;

                case Scope.NearbyAislingsExludingSelf:
                {
                    var nearby = GetObjects<Aisling>(Aisling.Map, i => i.WithinRangeOf(Aisling));

                    foreach (var obj in nearby)
                    {
                        if (obj.Serial == Aisling.Serial)
                            continue;

                        obj.Client.SendMessage(type, text);
                    }
                }
                    break;

                case Scope.AislingsOnSameMap:
                {
                    var nearby = GetObjects<Aisling>(Aisling.Map, i => i.WithinRangeOf(Aisling)
                                                                       && i.CurrentMapId == Aisling.CurrentMapId);

                    foreach (var obj in nearby)
                        obj.Client.SendMessage(type, text);
                }
                    break;

                case Scope.All:
                {
                    var nearby = GetObjects<Aisling>(null, i => i.LoggedIn);
                    foreach (var obj in nearby)
                        obj.Client.SendMessage(type, text);
                }
                    break;
            }
        }

        /// <summary>
        /// 이 맵의 곡을 알린다(0x19). 번호는 **16비트 그대로** 보낸다 — 예전에는 높은 바이트에 0xFF 를 넣고 낮은
        /// 바이트만 보내, 900 번대를 쓰는 맵 39장(화론마을 21장 …)이 132~135 로 잘려 **엉뚱한 곡**이 나왔다.
        /// 원작 클라이언트의 규칙은 값이 128 미만이면 효과음, 이상이면 (값 − 128) 번 곡, 228 이면 끄기다
        /// (Legend.exe 0x54c440). 효과음과 **같은 패킷(0x19)** 으로, 같은 길로 보낸다 — 예전에는 날바이트로
        /// 보내 암호를 거치지 않아 클라이언트가 아예 받지 못했다(2026-09-18 확인).
        /// </summary>
        public GameClient SendMusic()
        {
            if (Aisling.Map != null)
                Aisling.Client.Send(new ServerFormat19 { Number = (short) Aisling.Map.Music });

            return this;
        }

        public void SendOptionsDialog(Mundane mundane, string text, params OptionsDataItem[] options)
        {
            Send(new ServerFormat2F(mundane, text, new OptionsData(options)));
        }

        public void SendOptionsDialog(Mundane mundane, string text, string args, params OptionsDataItem[] options)
        {
            Send(new ServerFormat2F(mundane, text, new OptionsPlusArgsData(options, args)));
        }

        public void SendPopupDialog(Popup popup, string text, params OptionsDataItem[] options)
        {
            Send(new PopupFormat(popup, text, new OptionsData(options)));
        }

        public GameClient SendProfileUpdate()
        {
            Send(new byte[] {0b1001001, 0b0});

            return this;
        }

        public GameClient SendSerial()
        {
            Send(new ServerFormat05(Aisling));

            return this;
        }

        public void SendSkillForgetDialog(Mundane mundane, string text, ushort step)
        {
            Send(new ServerFormat2F(mundane, text, new SkillForfeitData(step)));
        }

        public void SendSkillLearnDialog(Mundane mundane, string text, ushort step, IEnumerable<SkillTemplate> skills)
        {
            Send(new ServerFormat2F(mundane, text, new SkillAcquireData(step, skills)));
        }

        public GameClient SendSound(byte sound, Scope scope = Scope.Self)
        {
            var empty = new ServerFormat13
            {
                Serial = Aisling.Serial,
                Health = byte.MaxValue,
                Sound = sound
            };

            Aisling.Show(scope, empty);
            return this;
        }

        public void SendSpellForgetDialog(Mundane mundane, string text, ushort step)
        {
            Send(new ServerFormat2F(mundane, text, new SpellForfeitData(step)));
        }

        public void SendSpellLearnDialog(Mundane mundane, string text, ushort step, IEnumerable<SpellTemplate> spells)
        {
            Send(new ServerFormat2F(mundane, text, new SpellAcquireData(step, spells)));
        }

        public GameClient SendStats(StatusFlags flags)
        {
            Send(new ServerFormat08(Aisling, flags));

            return this;
        }


        public GameClient SetAislingStartupVariables()
        {
            LastSave = DateTime.UtcNow;
            LastPingResponse = DateTime.UtcNow;
            PendingItemSessions = null;
            LastLocationSent = DateTime.UtcNow;
            LastMovement = DateTime.UtcNow;
            LastClientRefresh = DateTime.UtcNow;
            LastMessageSent = DateTime.UtcNow;
            BoardOpened = DateTime.UtcNow;
            Aisling.BonusAc = (100 - Aisling.Level / 3);
            Aisling.Exchange = null;
            Aisling.LastMapId = short.MaxValue;

            BuildSettings();
            return this;
        }

        public void ShowCurrentMenu(Sprite obj, MenuItem currentitem, MenuItem nextitem)
        {
            if (nextitem == null)
                return;

            nextitem.Text = nextitem.Text.Replace("%aisling%", Aisling.Username);

            switch (nextitem.Type)
            {
                case MenuItemType.Step:
                {
                    if (obj != null)
                        Send(new ReactorSequence(this, new DialogSequence
                        {
                            DisplayText = nextitem.Text,
                            HasOptions = false,
                            DisplayImage = (ushort) ((Mundane) obj).Template.Image,
                            Title = ((Mundane) obj).Template.Name,
                            CanMoveNext = nextitem.Answers.Length > 0,
                            CanMoveBack = nextitem.Answers.Any(i => i.Text == "back"),
                            Id = obj.Serial
                        }));
                    break;
                }
                case MenuItemType.Menu:
                {
                    if (obj != null)
                        SendOptionsDialog(obj as Mundane, nextitem.Text,
                            (from ans in nextitem.Answers
                                where ans.Text != "close"
                                select new OptionsDataItem((short) ans.Id, ans.Text)).ToArray());
                    break;
                }
            }
        }

        public void ShowCurrentMenu(Popup popup, MenuItem currentitem, MenuItem nextitem)
        {
            if (nextitem == null)
                return;

            nextitem.Text = nextitem.Text.Replace("%aisling%", Aisling.Username);

            switch (nextitem.Type)
            {
                case MenuItemType.Step:
                    Send(new ReactorSequence(this, new DialogSequence
                    {
                        DisplayText = nextitem.Text,
                        HasOptions = false,
                        DisplayImage = popup.Template.SpriteId,
                        Title = popup.Template.Name,
                        CanMoveNext = nextitem.Answers.Length > 0,
                        CanMoveBack = nextitem.Answers.Any(i => i.Text == "back"),
                        Id = popup.Id
                    }));
                    break;
                case MenuItemType.Menu:
                {
                    if (popup != null)
                        SendPopupDialog(popup, nextitem.Text,
                            (from ans in nextitem.Answers
                                where ans.Text != "close"
                                select new OptionsDataItem((short) ans.Id, ans.Text)).ToArray());
                    break;
                }
            }
        }

        /// <summary>무리에 있는 사람이 체력 0 이 되어 빠지는 혼수의 길이 — Novaonline `__SCRIPT_DEAD__` 의 `coma_delay @myid, 15`.</summary>
        public const int GroupComaSeconds = 15;

        public GameClient StatusCheck()
        {
            var proceed = false;

            if (Aisling.CurrentHp <= 0)
            {
                Aisling.CurrentHp = -1;
                proceed = true;
            }

            if (proceed)
            {
                Aisling.CurrentHp = 1;
                SendStats(StatusFlags.StructB);

                if (Aisling.Map.Flags.HasFlag(MapFlags.PlayerKill))
                {
                    for (var i = 0; i < 2; i++)
                        Aisling.RemoveBuffsAndDebuffs();

                    Aisling.CastDeath();

                    var target = Aisling.Target;

                    if (target != null)
                    {
                        if (target is Aisling)
                            SendMessage(Scope.NearbyAislings, 0x02,
                                Aisling.Username + "님이 " + (target as Aisling).Username + "님에게 죽었습니다.");
                    }
                    else
                    {
                        SendMessage(Scope.NearbyAislings, 0x02,
                            Aisling.Username + "님이 죽었습니다.");
                    }

                    return this;
                }

                if (!Aisling.Skulled)
                {
                    if (Aisling.CurrentMapId == ServerContext.Config.DeathMap)
                        return this;

                    // 5.99·Novaonline `__SCRIPT_DEAD__`: 무리가 있으면 혼수(`set_coma` · `coma_delay 15`) — 그 사이 무리가 살릴 수 있다.
                    // 무리가 없으면 혼수 없이 바로 죽어 뮤레칸의방으로 간다(`warp "뮤레칸의방", 10, 9` · `char_dead`). 무리 판정은 `group_exist` 와 같다.
                    if ((Aisling.PartyMembers?.Count ?? 0) <= 1 && !Aisling.GameMaster)
                    {
                        debuff_reeping.Die(Aisling);
                        return this;
                    }

                    var debuff = new debuff_reeping();
                    {
                        // 무리 혼수는 15초(coma_delay 15) — 설정 SkullLength(19)는 오솔길 함정 같은 다른 혼수에 그대로 둔다.
                        debuff.Timer.Tick = Math.Max(0, debuff.Length - GroupComaSeconds);
                        debuff.OnApplied(Aisling, debuff);
                    }
                }
            }

            return this;
        }

        public GameClient SystemMessage(string lpmessage)
        {
            SendMessage(0x02, lpmessage);
            return this;
        }

        public void TrainSkill(Skill skill)
        {
            if (skill.Level < skill.Template.MaxLevel)
            {
                var toImprove = (int) (0.10 / skill.Template.LevelRate);
                if (skill.Uses++ >= toImprove)
                {
                    skill.Level++;
                    skill.Uses = 0;
                    Send(new ServerFormat2C(skill.Slot, skill.Icon, skill.Name));

                    SendMessage(0x02, string.Format(CultureInfo.CurrentCulture, "{0}의 숙련도가 올랐습니다. (Lv. {1})",
                        skill.Template.Name,
                        skill.Level));
                }
            }

            Send(new ServerFormat3F(1,
                skill.Slot,
                (int)skill.Template.Cooldown));
        }

        public void TrainSpell(Spell spell)
        {
            if (spell.Level < spell.Template.MaxLevel)
            {
                var toImprove = (int) (0.10 / spell.Template.LevelRate);
                if (spell.Casts++ >= toImprove)
                {
                    spell.Level++;
                    spell.Casts = 0;
                    Send(new ServerFormat17(spell));
                    SendMessage(0x02,
                        string.Format(CultureInfo.CurrentCulture, "{0}의 숙련도가 올랐습니다.", spell.Template.Name));
                }
            }
        }

        public GameClient TransitionToMap(Area area, Position position)
        {
            if (area == null)
                return null;

            // 괴물이 선 칸에 내려놓으면 겹쳐 서서 그 괴물과는 싸울 수가 없다 — 빈 칸을 찾아 놓는다.
            position = area.FreeSpotNear(position);

            if (area.Id != Aisling.CurrentMapId)
            {
                LeaveArea(true, true);

                Aisling.LastPosition = new Position(Aisling.X, Aisling.Y);
                Aisling.XPos = position.X;
                Aisling.YPos = position.Y;
                Aisling.CurrentMapId = area.Id;

                EnterArea();
            }
            else
            {
                LeaveArea(true);

                Aisling.XPos = position.X;
                Aisling.YPos = position.Y;
                EnterArea();
            }

            Aisling.Client.CloseDialog();

            return this;
        }

        public GameClient TransitionToMap(int area, Position position)
        {
            if (ServerContext.GlobalMapCache.ContainsKey(area))
            {
                var target = ServerContext.GlobalMapCache[area];
                if (target != null)
                    TransitionToMap(target, position);
            }

            return this;
        }

        public void Update(TimeSpan elapsedTime)
        {
            #region Sanity Checks

            if (Aisling == null)
                return;

            if (!Aisling.LoggedIn)
                return;

            #endregion

            var distance = Aisling.Position.DistanceFrom(Aisling.LastPosition.X, Aisling.LastPosition.Y);

            if (distance > 2 && !IsWarping && (DateTime.UtcNow - LastMapUpdated).TotalMilliseconds > 2000)
            {
                LastWarp = DateTime.UtcNow;
                Aisling.LastPosition.X = (ushort) Aisling.XPos;
                Aisling.LastPosition.Y = (ushort) Aisling.YPos;
                LastLocationSent = DateTime.UtcNow;
                Refresh();
                return;
            }

            lock (_syncObj)
            {
                Aisling.SummonObjects?.Update(elapsedTime);

                if (Aisling.SummonObjects != null && !Aisling.SummonObjects.Spawns.Any())
                {
                    Aisling.SummonObjects = null;
                }
            }

            lock (Trap.Traps)
            {
                foreach (var trap in Trap.Traps.Select(i => i.Value))
                {
                    if (trap == null) continue;
                    trap.Update();

                    if (trap.Owner != null &&
                        trap.Owner.Serial != Aisling.Serial &&
                        Aisling.X == trap.Location.X &&
                        Aisling.Y == trap.Location.Y && Aisling.Map.Flags.HasFlag(MapFlags.PlayerKill))
                        Trap.Activate(trap, Aisling);
                }
            }

            if (!Aisling.GameMaster)
            {
                // 배열을 직접 찝지 않고 맵에 묻는다. 캐릭터는 제 맵 밖에 설 수 있다 — 저장된 자리는
                // 저장할 때의 맵 것이고, 더 작은 맵을 그 밑에 깔 때 그 자리가 아직 맞는지 아무도
                // 확인하지 않는다. 거기서 배열을 찝으면 **맥박마다** 예외가 난다(5분에 16,000번).
                // 예외 하나가 이 갱신의 나머지를 통째로 버리므로 세계가 도착하지 않는다 — 괴물도,
                // 상인도, 걸음도. 캐릭터는 빈 벌판에 굳고 클라이언트는 아무 말도 듣지 못한다.
                // Area.IsWall 은 바깥을 벽으로 답해서 캐릭터를 마지막 자리로 되돌린다.
                if (Aisling.Map.IsWall(Aisling.X, Aisling.Y))
                {
                    if (LastKnownPosition != null)
                    {
                        Aisling.X = LastKnownPosition.X;
                        Aisling.Y = LastKnownPosition.Y;
                    }

                    SendLocation();
                }
                else
                {
                    LastKnownPosition = new Position(Aisling.X, Aisling.Y);
                }
            }

            if ((DateTime.UtcNow - LastMessageFromClient).TotalSeconds > 120)
            {
                Aisling?.Remove(true);

                Server.ClientDisconnected(this);
            }

            DoUpdate(elapsedTime);
        }

        /// <summary>
        /// Runs whatever casts the client has asked for.
        /// </summary>
        /// <remarks>
        /// A cast naming a slot with no spell in it used to skip back to the top of this loop without taking
        /// anything off the stack, so the loop never ended: one bad cast held the thread that updates the
        /// world. A slot can empty between asking and casting, so it has to be dropped and moved past.
        ///
        /// The stack is also touched from the handler thread, so every use of it is now under the same lock
        /// that pushes to it.
        /// </remarks>
        private void DispatchCasts()
        {
            while (true)
            {
                CastInfo stack;

                lock (CastStack)
                {
                    if (!CastStack.Any())
                        return;

                    stack = CastStack.Peek();
                }

                var spell = Aisling.SpellBook.Get(i => i.Slot == stack.Slot).FirstOrDefault();

                if (spell == null)
                {
                    Drop(stack);
                    continue;
                }

                if (stack.Target == 0) stack.Target = (uint) Aisling.Serial;

                Aisling.CastSpell(spell);

                Drop(stack);
            }
        }

        /// <summary>Takes one cast off the stack, unless something else already did.</summary>
        private void Drop(CastInfo stack)
        {
            lock (CastStack)
            {
                if (CastStack.Any() && ReferenceEquals(CastStack.Peek(), stack))
                    CastStack.Pop();
            }
        }

        public GameClient UpdateDisplay()
        {
            var response = new ServerFormat33(Aisling);

            Aisling.Show(Scope.Self, response);

            var nearbyAislings = Aisling.AislingsNearby();

            if (!nearbyAislings.Any())
                return this;

            var myplayer = Aisling;
            foreach (var otherplayer in nearbyAislings)
            {
                if (otherplayer != null)
                {
                    if (myplayer.Serial == otherplayer.Serial)
                        continue;

                    if (!myplayer.Dead && !otherplayer.Dead)
                    {
                        if (myplayer.Invisible)
                            otherplayer.ShowTo(myplayer);
                        else
                            myplayer.ShowTo(otherplayer);

                        if (otherplayer.Invisible)
                            myplayer.ShowTo(otherplayer);
                        else
                            otherplayer.ShowTo(myplayer);
                    }
                    else
                    {
                        if (myplayer.Dead)
                            if (otherplayer.CanSeeGhosts())
                                myplayer.ShowTo(otherplayer);

                        if (otherplayer.Dead)
                            if (myplayer.CanSeeGhosts())
                                otherplayer.ShowTo(myplayer);
                    }
                }
            }

            return this;
        }

        public GameClient UpdateReactors(TimeSpan elapsedTime)
        {
            var inactive = new List<EphemeralReactor>();

            lock (Aisling.ActiveReactors)
            {
                var reactors = Aisling.ActiveReactors.Select(i => i.Value).ToArray();

                foreach (var reactor in reactors)
                {
                    reactor.Update(elapsedTime);

                    if (reactor.Expired) inactive.Add(reactor);
                }
            }

            foreach (var reactor in inactive.Where(reactor => Aisling.ActiveReactors.ContainsKey(reactor.YamlKey)))
                Aisling.ActiveReactors.Remove(reactor.YamlKey);

            return this;
        }

        public GameClient UpdateStatusBar(TimeSpan elapsedTime)
        {
            lock (_syncObj)
            {
                Aisling.UpdateBuffs(elapsedTime);
                Aisling.UpdateDebuffs(elapsedTime);
            }

            return this;
        }

        public void WarpTo(WarpTemplate warps)
        {
            if (warps.WarpType == WarpType.World)
                return;

            if (ServerContext.GlobalMapCache.Values.Any(i => i.Id == warps.ActivationMapId))
            {
                // 문구는 5.99 서버(Novaonline.exe 0x904b0 부근) 그대로다. 낮다는 말 뒤에는 몇 레벨부터인지를
                // 붙인다 — 5.99 는 말하지 않지만 모르면 몇 번이고 다시 부딪친다(사용자, 2026-09-24).
                if (!Aisling.GameMaster)
                {
                    if (warps.LevelRequired > 0 && Aisling.ExpLevel < warps.LevelRequired)
                    {
                        SendMessage(0x02, $"아직 들어가기엔 레벨이 낮습니다. (입장 레벨 {warps.LevelRequired})");
                        return;
                    }

                    if (warps.LevelMaximum > 0 && Aisling.ExpLevel > warps.LevelMaximum)
                    {
                        SendMessage(0x02, "이곳에 들어가기엔 늙었습니다.");
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(warps.ScriptNpc))
                {
                    var host = ServerContext.Game.ObjectFactory
                        .QueryAll<Mundane>(Aisling.Map, npc => npc.Template?.ScriptKey == warps.ScriptNpc).FirstOrDefault();

                    if (host?.Scripts != null)
                        foreach (var script in host.Scripts.Values)
                            script.OnClick(ServerContext.Game, this);

                    return;
                }

                if (warps.RequiresClear)
                {
                    var left = ServerContext.Game.ObjectFactory
                        .QueryAll<Monster>(Aisling.Map, monster => monster.CurrentHp > 0).Count();

                    if (left > 0)
                    {
                        // 5.99 서버 문구(Novaonline.exe 0x904b0 부근).
                        SendMessage(0x03, $"{{=q안내 : 이동할수 없습니다. [남아있는 몬스터수 : {left}]");
                        return;
                    }
                }

                if (Aisling.Map.Id != warps.To.AreaId)
                {
                    TransitionToMap(warps.To.AreaId, warps.To.Location);
                }
                else
                {
                    var landing = Aisling.Map.FreeSpotNear(warps.To.Location);

                    LeaveArea(true);
                    Aisling.XPos = landing.X;
                    Aisling.YPos = landing.Y;
                    EnterArea();
                    Aisling.Client.CloseDialog();
                }
            }
        }

        public void WarpTo(Position position)
        {
            var landing = Aisling.Map?.FreeSpotNear(position) ?? position;

            Aisling.XPos = landing.X;
            Aisling.YPos = landing.Y;

            Refresh();
        }

        private GameClient Enter()
        {
            // 맵 밖에 저장된 캐릭터를 안으로 들여놓는다. 예전 빌드가 내보낸 자리가 그대로 남아 있으면
            // 바닥도 괴물도 없는 곳에서 깨어나고, 스스로는 돌아올 길이 없다(2026-09-18).
            if (Aisling.Map != null)
            {
                if (Aisling.X < 0 || Aisling.Y < 0 || Aisling.X >= Aisling.Map.Cols || Aisling.Y >= Aisling.Map.Rows)
                {
                    Aisling.X = Math.Clamp(Aisling.X, 0, Math.Max(0, Aisling.Map.Cols - 1));
                    Aisling.Y = Math.Clamp(Aisling.Y, 0, Math.Max(0, Aisling.Map.Rows - 1));
                    SystemMessage("맵 밖에 있어 안으로 들어왔습니다.");
                }

                // 벽 속이나 사방이 막힌 구석에서 깨어나면 걸어 나올 길이 없다. 가장 가까운 트인 칸으로 꺼낸다.
                FreeIfStuck();
            }

            SendSerial();
            Insert();
            RefreshMap();
            UpdateDisplay();
            SendLocation();

            // 맵이 바뀔 때만 보내면 **다시 접속했을 때는 조용하다** — 나갈 때와 같은 맵이라 RefreshMap 의 조건에
            // 걸리지 않는다(2026-09-18 확인). 들어올 때도 한 번 알린다.
            SendMusic();

            return this;
        }
    }
}