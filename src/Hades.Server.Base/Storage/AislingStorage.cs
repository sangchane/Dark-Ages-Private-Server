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
        public static string StoragePath = $@"{ServerContext.StoragePath}/aislings";

        static AislingStorage()
        {
            if (!Directory.Exists(StoragePath))
                Directory.CreateDirectory(StoragePath);
        }

        public string[] Files => Directory.GetFiles(StoragePath, "*.json", SearchOption.TopDirectoryOnly);

        private const int DefaultMinNameLength = 2;
        private const int DefaultMaxNameLength = 12;

        // Precomposed Hangul syllables. Korean names must work: the protocol carries CP949 text, and the
        // mobile client sends them even though the English 7.18 client cannot type them.
        private const char FirstHangulSyllable = (char)0xAC00;
        private const char LastHangulSyllable = (char)0xD7A3;

        // The name becomes a file name, and these are devices on Windows rather than files.
        private static readonly string[] ReservedNames =
        {
            "con", "prn", "aux", "nul",
            "com1", "com2", "com3", "com4", "com5", "com6", "com7", "com8", "com9",
            "lpt1", "lpt2", "lpt3", "lpt4", "lpt5", "lpt6", "lpt7", "lpt8", "lpt9"
        };

        /// <summary>
        /// Letters, digits and Hangul only, within the configured length. The server accepted any name at
        /// all before this, and every name becomes a file.
        /// </summary>
        public static bool IsValidCharacterName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            var minimum = ServerContext.Config?.CharacterNameMinLength ?? DefaultMinNameLength;
            var maximum = ServerContext.Config?.CharacterNameMaxLength ?? DefaultMaxNameLength;

            if (name.Length < minimum || name.Length > maximum)
                return false;

            foreach (var character in name)
                if (!IsAllowedInName(character))
                    return false;

            return Array.IndexOf(ReservedNames, name.ToLowerInvariant()) < 0;
        }

        private static bool IsAllowedInName(char character)
        {
            return (character >= 'a' && character <= 'z')
                   || (character >= 'A' && character <= 'Z')
                   || (character >= '0' && character <= '9')
                   || (character >= FirstHangulSyllable && character <= LastHangulSyllable);
        }

        /// <summary>
        /// Resolves the file a character name maps to. Names arrive from the network and were used verbatim in
        /// the path, so a name containing a separator or a parent segment could place a file anywhere the
        /// server can write. Anything that does not resolve to a file directly inside the character directory
        /// is refused.
        /// </summary>
        private static string ResolveCharacterFile(string name)
        {
            if (!IsValidCharacterName(name))
                throw new ArgumentException($"'{name}' is not an allowed character name.", nameof(name));

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

                // Falls back to what the previous save left behind when the current file cannot be read.
                var text = SafeFile.Read(path, LooksLikeACharacter);

                if (text == null)
                    return null;

                var content = Encoding.ASCII.GetBytes(text);

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

        /// <summary>
        /// Whether a saved file is worth reading. A save that was cut short leaves something that starts
        /// like a character and stops in the middle, and that has to fall through to the backup.
        /// </summary>
        private static bool LooksLikeACharacter(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            try
            {
                return StorageManager.Deserialize<Aisling>(content) != null;
            }
            catch (Exception)
            {
                return false;
            }
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

                // Written beside the file and then swapped in, so a save that is interrupted leaves either
                // the previous character or the new one, never half of either.
                SafeFile.Write(path, objString);
            }
            catch (Exception ex)
            {
                ServerContext.Logger(ex.Message, Microsoft.Extensions.Logging.LogLevel.Error);
                ServerContext.Logger(ex.StackTrace, Microsoft.Extensions.Logging.LogLevel.Error);
            }
        }
    }
}