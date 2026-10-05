#region

using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Network.Game;

#endregion

namespace Darkages.Types
{
    public class Bank
    {
        [Newtonsoft.Json.JsonIgnore] public ActivitySession Activity { get; set; }
        public Bank()
        {
            Items = new Dictionary<string, Stack<Item>>();
        }

        public Dictionary<string, Stack<Item>> Items { get; set; }

        /// <summary>맡긴 금화. 캐릭터 파일(aislings json)에 함께 저장된다.</summary>
        public long Gold { get; set; }

        private static bool Stackable(Item item) => item?.Template?.Flags.HasFlag(ItemFlags.Stackable) == true;

        /// <summary>이 이름으로 맡긴 개수 — 겹치는 물건은 겹친 수까지 센다.</summary>
        public int Count(string name) => Items.TryGetValue(name, out var stack)
            ? stack.Sum(i => Stackable(i) ? Math.Max(1, (int) i.Stacks) : 1)
            : 0;

        /// <summary>겹치는 물건 <paramref name="count" />개를 맡긴다 — 맨 위 묶음에 더하고, ushort 를 넘으면 새 묶음.</summary>
        public void DepositStacks(Sprite owner, Item like, int count)
        {
            using var mutation = ActivitySession.BeginMutation(Activity, "Bank.Deposit");
            if (!Items.TryGetValue(like.DisplayName, out var stack))
                Items[like.DisplayName] = stack = new Stack<Item>();

            if (stack.Count > 0 && Stackable(stack.Peek()) && stack.Peek().Stacks + count <= ushort.MaxValue)
            {
                stack.Peek().Stacks = (ushort) (stack.Peek().Stacks + count);
            }
            else
            {
                var copy = Item.Create(owner, like.Template);
                copy.Stacks = (ushort) count;
                stack.Push(copy);
            }

            Activity?.ItemsChanged();
        }

        public void Deposit(Item lpItem)
        {
            using var mutation = ActivitySession.BeginMutation(Activity, "Bank.Deposit");
            if (!Items.ContainsKey(lpItem.DisplayName))
            {
                Items[lpItem.DisplayName] = new Stack<Item>();
            }

            Items[lpItem.DisplayName].Push(lpItem);
            Activity?.ItemsChanged();
        }

        /// <summary>
        /// <paramref name="count" />개를 가방에 넣고, 들어간 뒤에만 은행에서 뺀다 — 가방이 차 있으면 아무것도 바뀌지 않는다.
        /// 겹치는 물건은 한 번에 한 칸 한도(MaxStack)까지.
        /// </summary>
        public bool Withdraw(IGameClient client, string itemName, int count = 1)
        {
            using var mutation = ActivitySession.BeginMutation(Activity, "Bank.Withdraw");
            if (!Items.TryGetValue(itemName, out var stack) || stack.Count == 0)
                return false;

            var top = stack.Peek();
            if (top?.Template?.Name != null &&
                ServerContext.GlobalItemTemplateCache.TryGetValue(top.Template.Name, out var template))
                top.Template = template;

            if (Stackable(top))
            {
                var cap = top.Template.MaxStack > 0 ? top.Template.MaxStack : ushort.MaxValue;
                if (count < 1 || count > Count(itemName) || count > cap)
                    return false;

                var given = Item.Create(client.Aisling, top.Template);
                given.Stacks = (ushort) count;
                if (!given.GiveTo(client.Aisling))
                    return false;

                for (var left = count; left > 0;)
                {
                    var bundle = stack.Peek();
                    var held = Math.Max(1, (int) bundle.Stacks);
                    if (held <= left)
                    {
                        stack.Pop();
                        left -= held;
                    }
                    else
                    {
                        bundle.Stacks = (ushort) (held - left);
                        left = 0;
                    }
                }
            }
            else
            {
                if (count != 1 || !top.GiveTo(client.Aisling))
                    return false;

                stack.Pop();
            }

            if (stack.Count == 0)
                Items.Remove(itemName);

            Activity?.ItemsChanged();
            return true;
        }
    }
}