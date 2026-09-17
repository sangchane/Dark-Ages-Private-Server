using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 5.99 NPC 스크립트(`db/script/Npc/Npc_*.txt`)를 적힌 그대로 돌린다. `scripts/build-pack-npcs.py` 가 스크립트마다
    /// 이것을 상속한 클래스를 만들고, 문장은 기술·마법과 같은 변환기로 C# 이 된다 — 명령은 <see cref="Pack599.Call" /> 로 간다.
    /// </summary>
    /// <remarks>
    /// NPC 스크립트는 기술과 달리 **중간에 플레이어를 기다린다.** `mes 1, "…"` 은 말하고 "다음"을 누를 때까지, `menu(…)` 는
    /// 고를 때까지 멈춘다. 그래서 대화는 C# 이터레이터로 옮겨지고(<c>yield return</c> 이 멈추는 자리), 누른 답이 오면
    /// (<see cref="OnResponse" />) 멈춘 자리에서 이어 간다. `mes 0` 은 끝맺는 말이라 기다리지 않는다 — 뒤에 적힌 문장
    /// (배우기 따위)은 곧장 돈다. 창을 닫으면 대화는 그대로 버려지고, 다시 누르면 처음부터다.
    /// <para>
    /// 대화는 NPC 스크립트 인스턴스 하나(서버가 켜져 있는 내내 산다)에 캐릭터 번호로 쌓인다. 끝까지 가지 않고 창을 닫거나
    /// 접속을 끊은 대화는 스스로 지워지지 않고, 캐릭터 번호는 접속마다 새로 나오므로 다시 들어와도 치워지지 않는다 —
    /// 그래서 누가 말을 걸 때마다 접속이 끊긴 번호를 치운다. 스크립트가 도중에 예외를 내면 그 대화를 버리고 창을 닫는다
    /// (남겨 두면 반쯤 돈 대화가 박혀 그 NPC 와 다시 말할 수 없다).
    /// </para>
    /// </remarks>
    public abstract class PackNpc : MundaneScript
    {
        private readonly ConcurrentDictionary<int, (IEnumerator<Prompt> Steps, Reply Reply)> _talking =
            new ConcurrentDictionary<int, (IEnumerator<Prompt>, Reply)>();

        protected PackNpc(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        /// <summary>스크립트 본문. 말하거나 물을 때마다 <see cref="Prompt" /> 하나를 내놓고 답을 기다린다.</summary>
        protected abstract IEnumerable<Prompt> Talk(Pack599 p, Reply reply);

        public override void OnClick(GameServer server, GameClient client)
        {
            ForgetTheGone(server);

            var reply = new Reply();
            _talking[client.Aisling.Serial] = (Talk(new Pack599(client.Aisling, Mundane), reply).GetEnumerator(), reply);
            Advance(client);
        }

        public override void OnResponse(GameServer server, GameClient client, ushort responseId, string args)
        {
            if (!_talking.TryGetValue(client.Aisling.Serial, out var talk))
                return;

            talk.Reply.Choice = responseId;
            talk.Reply.Words = args ?? "";
            Advance(client);
        }

        public override void OnGossip(GameServer server, GameClient client, string message)
        {
        }

        public override void TargetAcquired(Sprite target)
        {
        }

        private void Advance(GameClient client)
        {
            var steps = _talking[client.Aisling.Serial].Steps;

            try
            {
                while (steps.MoveNext())
                {
                    var prompt = steps.Current;
                    if (prompt.Typing)
                        client.Send(new ServerFormat2F(Mundane, prompt.Text, new TextInputData {Step = 1}));
                    else
                        client.SendOptionsDialog(Mundane, prompt.Text,
                            prompt.Choices.Select((choice, i) => new OptionsDataItem((short) (i + 1), choice)).ToArray());

                    if (prompt.Waits)
                        return;
                }
            }
            catch (Exception error)
            {
                ServerContext.Logger($"[5.99] {Mundane.Template?.Name} 대화가 도중에 멈췄습니다: {error.Message}");
                client.CloseDialog();
            }

            _talking.TryRemove(client.Aisling.Serial, out _);
        }

        /// <summary>접속이 끊긴 캐릭터 번호의 대화를 치운다.</summary>
        private void ForgetTheGone(GameServer server)
        {
            var online = server.Clients.Where(c => c?.Aisling != null).Select(c => c.Aisling.Serial).ToHashSet();

            foreach (var serial in _talking.Keys.Where(serial => !online.Contains(serial)).ToList())
                _talking.TryRemove(serial, out _);
        }

        /// <summary>`mes 종류, 글`. 종류 1 은 "다음"을 누를 때까지 기다리고, 0 은 끝맺는 말이라 기다리지 않는다.</summary>
        protected static Prompt Mes(V kind, V text) =>
            kind.Num == 1 ? new Prompt(text, new[] {"다음"}, true) : new Prompt(text, new string[0], false);

        /// <summary>`menu(물음, 고를 것…)`. 고른 것은 1부터 센 번호로 <see cref="Reply.Choice" /> 에 온다.</summary>
        protected static Prompt Menu(V question, params V[] choices) =>
            new Prompt(question, choices.Select(choice => choice.ToString()).ToArray(), true);

        /// <summary>`input @글$, 종류, "물음", …`. 쳐 넣은 글은 <see cref="Reply.Words" /> 에 온다.</summary>
        protected static Prompt Input(V question) => new Prompt(question, new string[0], true, typing: true);

        protected sealed class Prompt
        {
            public Prompt(V text, string[] choices, bool waits, bool typing = false)
            {
                // 팩 글은 줄바꿈을 `\n` 두 글자로 적는다.
                Text = text.ToString().Replace("\\n", "\n");
                Choices = choices;
                Waits = waits;
                Typing = typing;
            }

            public string Text { get; }
            public string[] Choices { get; }
            public bool Waits { get; }
            public bool Typing { get; }
        }

        protected sealed class Reply
        {
            public V Choice;
            public V Words = "";
        }
    }
}
