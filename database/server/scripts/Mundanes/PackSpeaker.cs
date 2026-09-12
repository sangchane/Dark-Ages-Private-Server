#region

using System.Linq;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    /// <summary>
    /// Says what the pack says this NPC says, and nothing else.
    /// </summary>
    /// <remarks>
    /// The pack gives 31 NPCs a list of lines and no menu. `simple_generic` cannot serve them: it reads a
    /// YAML menu from a folder that does not exist, so clicking one opens nothing at all. Rather than leave
    /// every ported NPC mute — or guess which of the 25 hand-written scripts each one wants — this shows
    /// the lines that came with the data. Whoever later finds the real script for an NPC replaces its
    /// ScriptKey; until then the world has NPCs that answer.
    /// </remarks>
    [Script("pack_speaker")]
    public class PackSpeaker : MundaneScript
    {
        public PackSpeaker(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        public override void OnClick(GameServer server, GameClient client)
        {
            client.SendOptionsDialog(Mundane, Lines());
        }

        public override void OnGossip(GameServer server, GameClient client, string message)
        {
        }

        public override void OnResponse(GameServer server, GameClient client, ushort responseId, string args)
        {
        }

        public override void TargetAcquired(Sprite target)
        {
        }

        /// <summary>
        /// One box, every line. A client that is told nothing shows an empty window, which reads as a bug
        /// rather than as an NPC with nothing to say — so say something.
        /// </summary>
        private string Lines()
        {
            var speech = Mundane.Template?.Speech?
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToArray();

            return speech is { Length: > 0 }
                ? string.Join("\n", speech)
                : $"{Mundane.Template?.Name} 은(는) 아무 말도 하지 않는다.";
        }
    }
}
