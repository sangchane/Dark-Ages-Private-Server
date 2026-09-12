#region

using System;
using System.IO;
using System.Text.Json;
using Darkages.Types;


#endregion

namespace Darkages.Storage
{
    public class WarpStorage : IStorage<WarpTemplate>
    {
        public static string StoragePath;

        static WarpStorage()
        {
            StoragePath = $@"{ServerContext.StoragePath}/templates/warps";

            if (!Directory.Exists(StoragePath))
                Directory.CreateDirectory(StoragePath);
        }

        public void CacheFromStorage()
        {
            var areaDir = StoragePath;
            if (!Directory.Exists(areaDir))
                return;

            var areaNames = Directory.GetFiles(areaDir, "*.json", SearchOption.TopDirectoryOnly);

            foreach (var area in areaNames)
            {
                var obj = StorageManager.WarpBucket.Load(Path.GetFileNameWithoutExtension(area));

                // Adding whatever came back made the count the number of files, always — a check that
                // cannot go red — and left nulls in the list for something else to trip over later.
                if (obj == null)
                {
                    ServerContext.Logger(
                        $"Warp {Path.GetFileName(area)} could not be read. Not loaded.",
                        Microsoft.Extensions.Logging.LogLevel.Error);

                    continue;
                }

                ServerContext.GlobalWarpTemplateCache.Add(obj);
            }
        }

        public WarpTemplate Load(string name)
        {
            var path = Path.Combine(StoragePath, $"{name.ToLower()}.json");

            if (!File.Exists(path))
                return null;

            // One warp that does not parse used to end startup for the whole server: Deserialize throws
            // and nothing here caught it. A file we cannot read is a file to name and skip.
            try
            {
                using var s = File.OpenRead(path);
                using var f = new StreamReader(s);
                return StorageManager.Deserialize<WarpTemplate>(f.ReadToEnd());
            }
            catch (Exception ex)
            {
                ServerContext.Logger(ex.Message, Microsoft.Extensions.Logging.LogLevel.Error);
                return null;
            }
        }

        public void Save(WarpTemplate obj)
        {
            var path = Path.Combine(StoragePath, $"{obj.Name.ToLower()}.json");
            var objString = StorageManager.Serialize(obj);
            File.WriteAllText(path, objString);
        }
    }
}