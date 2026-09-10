using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace Darkages.Storage
{
    /// <summary>
    /// Replaces a file's contents in one step, keeping the previous contents beside it.
    /// </summary>
    /// <remarks>
    /// Writing over a file empties it first and then fills it, so a process that stops in between leaves
    /// nothing behind — for a character file that is the character. Writing the new contents somewhere else
    /// and then swapping the two means a reader sees either all of the old or all of the new, and the old is
    /// still there afterwards if the new turns out to be unreadable.
    /// </remarks>
    public static class SafeFile
    {
        private const string WritingSuffix = ".writing";
        private const string BackupSuffix = ".backup";

        // One writer at a time per file. Two saves of the same character used to interleave.
        private static readonly ConcurrentDictionary<string, object> Writers =
            new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Where the previous contents of a file are kept.</summary>
        public static string BackupPath(string path) => path + BackupSuffix;

        public static void Write(string path, string content)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            lock (Writers.GetOrAdd(path, _ => new object()))
            {
                var temporary = path + WritingSuffix;

                // All of it, and on the disk, before anything is swapped.
                using (var stream = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                {
                    writer.Write(content);
                    writer.Flush();
                    stream.Flush(true);
                }

                if (File.Exists(path))
                    File.Replace(temporary, path, BackupPath(path), true);
                else
                    File.Move(temporary, path);
            }
        }

        /// <summary>
        /// Reads the file, or its backup when the file is missing or unreadable. Returns null when neither
        /// can be read.
        /// </summary>
        public static string Read(string path, Func<string, bool> readable)
        {
            var current = TryRead(path);

            if (current != null && (readable == null || readable(current)))
                return current;

            var previous = TryRead(BackupPath(path));

            if (previous != null && (readable == null || readable(previous)))
                return previous;

            return null;
        }

        private static string TryRead(string path)
        {
            try
            {
                return File.Exists(path) ? File.ReadAllText(path) : null;
            }
            catch (IOException)
            {
                return null;
            }
        }
    }
}
