#region

using System;
using System.IO;
using System.Text;
using System.Text.Json;

#endregion

namespace Darkages.Storage
{
    public class AislingStorage : IStorage<Aisling>
    {
        public static string StoragePath = $@"{ServerContext.StoragePath}\aislings";

        static AislingStorage()
        {
            if (!Directory.Exists(StoragePath))
                Directory.CreateDirectory(StoragePath);
        }

        public string[] Files => Directory.GetFiles(StoragePath, "*.json", SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Resolves the file a character name maps to. Names arrive from the network and were used verbatim in
        /// the path, so a name containing a separator or a parent segment could place a file anywhere the
        /// server can write. Anything that does not resolve to a file directly inside the character directory
        /// is refused.
        /// </summary>
        private static string ResolveCharacterFile(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("A character name cannot be empty.", nameof(name));

            var resolved = Path.GetFullPath(Path.Combine(StoragePath, $"{name.ToLower()}.json"));

            if (!string.Equals(Path.GetDirectoryName(resolved), Path.GetFullPath(StoragePath),
                    StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(
                    "A character name must resolve to a file directly inside the character directory.",
                    nameof(name));

            return resolved;
        }

        public Aisling Load(string name)
        {
            try
            {
                var path = ResolveCharacterFile(name);

                if (!File.Exists(path))
                    return null;

                var content = File.ReadAllBytes(path);

                // ReSharper disable UseIndexFromEndExpression
                if (content[content.Length - 1] == 0x7D && content[content.Length - 3] == 0x7D)
                {
                    content[content.Length - 3] = 0x7D;
                    content[content.Length - 2] = 0x20;
                    content[content.Length - 1] = 0x20;
                }

                var jsoncontent = Encoding.ASCII.GetString(content);
                var aisling = StorageManager.Deserialize<Aisling>(jsoncontent);

                return aisling;
            }
            catch (Exception ex)
            {
                ServerContext.Logger(ex.Message, Microsoft.Extensions.Logging.LogLevel.Error);
                ServerContext.Logger(ex.StackTrace, Microsoft.Extensions.Logging.LogLevel.Error);
                ServerContext.Logger($"Error : {ex.Message}. Aisling could not be loaded.", Microsoft.Extensions.Logging.LogLevel.Error);
            }

            return null;
        }

        public void Save(Aisling obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (ServerContext.Config.DontSavePlayers) return;

            try
            {
                var path = ResolveCharacterFile(obj.Username);
                var objString = StorageManager.Serialize(obj);

                File.WriteAllText(path, objString);
            }
            catch (Exception ex)
            {
                ServerContext.Logger(ex.Message, Microsoft.Extensions.Logging.LogLevel.Error);
                ServerContext.Logger(ex.StackTrace, Microsoft.Extensions.Logging.LogLevel.Error);
            }
        }
    }
}