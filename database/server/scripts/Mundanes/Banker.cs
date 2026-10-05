#region

using System;
using System.Collections.Concurrent;
using System.Linq;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Scripts.Mundanes.LORULE_CITY.Bankers
{
    /// <summary>
    /// 은행(오리아나) — 물건·금화를 맡기고 찾는다. 맡긴 것은 <see cref="Aisling.BankManager" />(캐릭터 파일)에 남는다.
    /// 앱이 이미 그리는 창만 쓴다: 메뉴(0x00) · 가방 칸 고르기(0x05) · 맡긴 목록(0x04) · 수 입력(0x02).
    /// 모든 검사는 바꾸기 직전, 은행 잠금 안에서 다시 한다 — 연타·두 창이 겹쳐도 복사·소실이 없다.
    /// </summary>
    [Script("Banker")]
    public class Merchant : MundaneScript
    {
        private const ushort DepositItem = 0x01, WithdrawItem = 0x02, DepositGold = 0x03, WithdrawGold = 0x04;
        private const ushort DepositPicked = 0x0800, DepositCount = 0x0801; // BankingData 는 번호를 한 바이트(0x08)만 쓴다
        private const ushort WithdrawPicked = 0x000A, WithdrawCount = 0x000B;
        private const ushort DepositGoldAmount = 0x0030, WithdrawGoldAmount = 0x0040;
        private const ushort BulkWithdraw = 0x0F01, BulkDeposit = 0x0F02; // 앱의 상점식 일괄 창(0xF2, GameServerHandlers.FormatF2Handler)

        private static readonly OptionsDataItem[] Choices =
        {
            new OptionsDataItem((short) DepositItem, "물건 맡기기"),
            new OptionsDataItem((short) WithdrawItem, "물건 찾기 (맡긴 목록)"),
            new OptionsDataItem((short) DepositGold, "금화 맡기기"),
            new OptionsDataItem((short) WithdrawGold, "금화 찾기")
        };

        // 수를 묻는 창을 띄운 동안 무엇에 대한 수인지 — 캐릭터 번호 → 가방 칸(맡기기) 또는 물건 이름(찾기).
        private readonly ConcurrentDictionary<int, (int Slot, string Name)> _asking = new();

        public Merchant(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        public override void OnClick(GameServer server, GameClient client)
        {
            var bank = client.Aisling.BankManager ??= new Bank();
            client.SendOptionsDialog(Mundane,
                $"무엇을 도와드릴까요?\n맡긴 금화 {bank.Gold:N0}전 · 맡긴 물건 {bank.Items.Count}종",
                Choices);
        }

        public override void OnGossip(GameServer server, GameClient client, string message)
        {
        }

        public override void TargetAcquired(Sprite Target)
        {
        }

        public override void OnResponse(GameServer server, GameClient client, ushort responseID, string args)
        {
            var aisling = client.Aisling;
            if (aisling == null || !aisling.LoggedIn || aisling.Dead)
                return;

            if (!aisling.WithinRangeOf(Mundane))
            {
                client.CloseDialog();
                return;
            }

            var bank = aisling.BankManager ??= new Bank();

            // 한 사람의 은행 일은 한 번에 하나 — 같은 요청이 두 번 와도 두 번째는 바뀐 상태를 보고 판단한다.
            lock (bank)
            {
                using var mutation = ActivitySession.BeginMutation(client.Activity, "Bank");
                switch (responseID)
                {
                    case DepositItem:
                        DepositMenu(client);
                        break;
                    case WithdrawItem:
                        WithdrawMenu(client, bank);
                        break;
                    case DepositGold:
                        Ask(client, $"얼마를 맡기시겠습니까? (가진 금화 {aisling.GoldPoints:N0}전)", DepositGoldAmount);
                        break;
                    case WithdrawGold:
                        Ask(client, $"얼마를 찾으시겠습니까? (맡긴 금화 {bank.Gold:N0}전)", WithdrawGoldAmount);
                        break;

                    case DepositPicked:
                    {
                        var item = Bankable(client, Number(args));
                        if (item == null)
                        {
                            DepositMenu(client, "그 물건은 맡을 수 없습니다.");
                            break;
                        }

                        if (Stackable(item) && item.Stacks > 1)
                        {
                            _asking[aisling.Serial] = (item.Slot, item.Template.Name);
                            Ask(client, $"{item.DisplayName} 몇 개를 맡기시겠습니까? (1~{item.Stacks})", DepositCount);
                            break;
                        }

                        Deposit(client, bank, item.Slot, item.Template.Name, 1);
                        break;
                    }

                    case DepositCount:
                        if (_asking.TryRemove(aisling.Serial, out var deposit) && deposit.Name != null)
                            Deposit(client, bank, deposit.Slot, deposit.Name, Number(args));
                        else
                            DepositMenu(client);
                        break;

                    case WithdrawPicked:
                    {
                        var name = args ?? string.Empty;
                        var held = bank.Count(name);
                        if (held <= 0)
                        {
                            WithdrawMenu(client, bank, "맡기신 물건이 없습니다.");
                            break;
                        }

                        if (Stackable(bank.Items[name].Peek()) && held > 1)
                        {
                            _asking[aisling.Serial] = (0, name);
                            Ask(client, $"{name} 몇 개를 찾으시겠습니까? (맡긴 수 {held})", WithdrawCount);
                            break;
                        }

                        Withdraw(client, bank, name, 1);
                        break;
                    }

                    case WithdrawCount:
                        if (_asking.TryRemove(aisling.Serial, out var withdraw) && withdraw.Name != null)
                            Withdraw(client, bank, withdraw.Name, Number(args));
                        else
                            WithdrawMenu(client, bank);
                        break;

                    case DepositGoldAmount:
                    {
                        var amount = Number(args);
                        if (amount < 1 || amount > aisling.GoldPoints)
                        {
                            Say(client, $"1전부터 가진 금화({aisling.GoldPoints:N0}전)까지만 맡길 수 있습니다.");
                            break;
                        }

                        aisling.GoldPoints -= amount;
                        bank.Gold += amount;
                        client.Activity?.Currency("bank_gold", amount, bank.Gold);
                        client.SendStats(StatusFlags.StructC);
                        Say(client, $"금화 {amount:N0}전을 맡았습니다. 맡긴 금화 {bank.Gold:N0}전");
                        break;
                    }

                    case WithdrawGoldAmount:
                    {
                        var amount = Number(args);
                        var room = (long) ServerContext.Config.MaxCarryGold - aisling.GoldPoints;
                        if (amount < 1 || amount > bank.Gold)
                        {
                            Say(client, $"1전부터 맡긴 금화({bank.Gold:N0}전)까지만 찾을 수 있습니다.");
                            break;
                        }

                        if (amount > room)
                        {
                            Say(client, $"더 들 수 없습니다. 지금은 {Math.Max(0, room):N0}전까지 찾을 수 있습니다.");
                            break;
                        }

                        bank.Gold -= amount;
                        aisling.GoldPoints += amount;
                        client.Activity?.Currency("bank_gold", -amount, bank.Gold);
                        client.SendStats(StatusFlags.StructC);
                        Say(client, $"금화 {amount:N0}전을 돌려드렸습니다. 맡긴 금화 {bank.Gold:N0}전");
                        break;
                    }

                    case BulkDeposit:
                    {
                        // 줄마다 "칸\t수". 수는 가진 만큼으로 줄인다(전량 맡기기). 같은 칸이 두 줄이어도 두 번째는 남은 것만.
                        int kinds = 0, total = 0;
                        foreach (var (key, count) in Lines(args))
                        {
                            var item = Bankable(client, Number(key));
                            if (item == null)
                                continue;

                            var held = Stackable(item) ? Math.Max(1, (int) item.Stacks) : 1;
                            var moved = Math.Min(count, held);
                            if (Put(client, bank, item.Slot, item.Template.Name, moved))
                            {
                                kinds++;
                                total += moved;
                            }
                        }

                        DepositMenu(client, kinds > 0 ? $"{kinds}종 {total}개를 맡았습니다." : "맡길 수 있는 물건을 고르십시오.");
                        break;
                    }

                    case BulkWithdraw:
                    {
                        // 줄마다 "이름\t수". 맡긴 만큼으로 줄이고, 한 칸 한도씩 나눠 가방에 들어가는 만큼만 꺼낸다.
                        int kinds = 0, total = 0;
                        var full = false;
                        foreach (var (name, count) in Lines(args))
                        {
                            var left = Math.Min(count, bank.Count(name));
                            var given = 0;
                            while (left > 0)
                            {
                                var top = bank.Items[name].Peek();
                                var chunk = Stackable(top) ? Math.Min(left, top.Template.MaxStack > 0 ? top.Template.MaxStack : ushort.MaxValue) : 1;
                                if (!bank.Withdraw(client, name, chunk))
                                {
                                    full = true;
                                    break;
                                }

                                given += chunk;
                                left -= chunk;
                            }

                            if (given > 0)
                            {
                                kinds++;
                                total += given;
                            }
                        }

                        WithdrawMenu(client, bank, (kinds > 0 ? $"{kinds}종 {total}개를 돌려드렸습니다." : "")
                                                   + (full ? (kinds > 0 ? " " : "") + "가방에 자리가 없어 나머지는 맡아 둡니다." : "")
                                                   + (kinds == 0 && !full ? "찾을 물건을 고르십시오." : ""));
                        break;
                    }

                    default:
                        client.CloseDialog();
                        break;
                }
            }
        }

        private static bool Stackable(Item item) => item?.Template?.Flags.HasFlag(ItemFlags.Stackable) == true;

        /// <summary>앱이 보낸 수. 숫자가 아니거나 범위를 넘으면 0 — 0 은 어느 검사도 통과하지 못한다.</summary>
        private static int Number(string args) =>
            int.TryParse(args?.Trim().Replace(",", ""), out var value) && value > 0 ? value : 0;

        /// <summary>일괄 창의 줄들("이름 또는 칸\t수"). 수가 1 미만인 줄은 버린다.</summary>
        private static (string Key, int Count)[] Lines(string args) =>
            (args ?? string.Empty).Split('\n')
            .Select(line => line.Split('\t'))
            .Where(parts => parts.Length == 2 && Number(parts[1]) > 0)
            .Select(parts => (parts[0], Number(parts[1])))
            .Take(128)
            .ToArray();

        private static Item Bankable(GameClient client, int slot) =>
            slot <= 0 ? null : client.Aisling.Inventory.FindInSlot(slot) is { Template: { } template } item
                               && template.Flags.HasFlag(ItemFlags.Bankable)
                ? item
                : null;

        private void Deposit(GameClient client, Bank bank, int slot, string name, int count)
        {
            if (Put(client, bank, slot, name, count))
                DepositMenu(client, $"{name} {count}개를 맡았습니다.");
            else
                DepositMenu(client, "맡길 수 없습니다. 가방을 다시 확인해 주세요.");
        }

        /// <summary>그 칸에 지금도 같은 물건이 <paramref name="count" />개 이상 있을 때만 — 가방에서 먼저 빼고 은행에 넣는다.</summary>
        private static bool Put(GameClient client, Bank bank, int slot, string name, int count)
        {
            var item = Bankable(client, slot);
            if (item == null || item.Template.Name != name || count < 1 ||
                count > (Stackable(item) ? Math.Max(1, (int) item.Stacks) : 1))
                return false;

            if (Stackable(item))
            {
                client.Aisling.Inventory.RemoveRange(client, item, count);
                bank.DepositStacks(client.Aisling, item, count);
                return true;
            }

            if (client.Aisling.Inventory.Remove((byte) item.Slot) != item)
                return false;

            client.Send(new ServerFormat10((byte) item.Slot));
            client.Aisling.CurrentWeight = Math.Max(0, client.Aisling.CurrentWeight - item.Template.CarryWeight);
            client.SendStats(StatusFlags.StructA);
            bank.Deposit(item);
            return true;
        }

        private void Withdraw(GameClient client, Bank bank, string name, int count)
        {
            if (count < 1 || count > bank.Count(name))
            {
                WithdrawMenu(client, bank, "그만큼 맡기신 것이 없습니다.");
                return;
            }

            WithdrawMenu(client, bank, bank.Withdraw(client, name, count)
                ? $"{name} {count}개를 돌려드렸습니다."
                : "가방에 자리가 없거나 한 번에 찾을 수 있는 수를 넘었습니다.");
        }

        private void DepositMenu(GameClient client, string said = null)
        {
            var slots = client.Aisling.Inventory.BankList.ToArray();
            if (slots.Length == 0)
            {
                Say(client, (said == null ? "" : said + "\n") + "맡길 수 있는 물건이 없습니다.");
                return;
            }

            client.Send(new ServerFormat2F(Mundane, said ?? "무엇을 맡기시겠습니까?",
                new BankingData((ushort) (DepositPicked >> 8), slots)));
        }

        private void WithdrawMenu(GameClient client, Bank bank, string said = null)
        {
            if (bank.Items.Count == 0)
            {
                Say(client, (said == null ? "" : said + "\n") + "맡기신 물건이 없습니다.");
                return;
            }

            client.Send(new ServerFormat2F(Mundane, said ?? "무엇을 찾으시겠습니까?",
                new WithdrawBankData(WithdrawPicked, bank)));
        }

        private void Ask(GameClient client, string question, ushort step) =>
            client.Send(new ServerFormat2F(Mundane, question, new TextInputData {Step = step}));

        private void Say(GameClient client, string words) =>
            client.SendOptionsDialog(Mundane, words,
                Choices);
    }
}
