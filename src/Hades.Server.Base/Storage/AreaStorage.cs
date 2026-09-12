#region

using Darkages.IO;
using Darkages.Scripting;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

#endregion

namespace Darkages.Storage
{
    public class AreaStorage : IStorage<Area>
    {
        public static string StoragePath;

        static AreaStorage()
        {
            StoragePath = $@"{ServerContext.StoragePath}/areas";

            if (!Directory.Exists(StoragePath))
                Directory.CreateDirectory(StoragePath);
        }

        public int Count => Directory.GetFiles(StoragePath, "*.json", SearchOption.TopDirectoryOnly).Length;

        /// <summary>Six bytes a tile: a floor and two walls, each a ushort.</summary>
        private const int BytesPerTile = 6;

        public static bool LoadMap(Area mapObj, string mapFile, bool save = false)
        {
            var bytes = File.ReadAllBytes(mapFile);
            var expected = mapObj.Cols * mapObj.Rows * BytesPerTile;

            // A map short of its own size does not fail to load — the reader fills what is missing with
            // wall and reports success all the same, so an empty or truncated file becomes a sealed room
            // that nothing complains about, cached and counted towards Map Templates Loaded. The size is
            // known exactly from the area's own dimensions, so say so instead of guessing at wall.
            if (bytes.Length != expected)
            {
                ServerContext.Logger(
                    $"Map {mapObj.Id} ({mapObj.Name}): {Path.GetFileName(mapFile)} is {bytes.Length} bytes, " +
                    $"expected {expected} for {mapObj.Cols}x{mapObj.Rows}. Not loaded.",
                    Microsoft.Extensions.Logging.LogLevel.Error);

                return false;
            }

            mapObj.FilePath = mapFile;
            mapObj.Data = bytes;
            mapObj.Hash = Crc16Provider.ComputeChecksum(mapObj.Data);
            {
                if (save) StorageManager.AreaBucket.Save(mapObj);
            }

            return mapObj.OnLoaded();
        }

        public void CacheFromStorage()
        {
            var areaDir = StoragePath;
            if (!Directory.Exists(areaDir))
                return;

            var areaNames = Directory.GetFiles(areaDir, "*.json", SearchOption.TopDirectoryOnly);

            foreach (var area in areaNames)
            {
                var mapObj = StorageManager.AreaBucket.Load(Path.GetFileNameWithoutExtension(area));

                if (mapObj == null)
                    continue;

                var mapFile = Directory.GetFiles($@"{ServerContext.StoragePath}/maps", $"lod{mapObj.Id}.map",
                    SearchOption.TopDirectoryOnly).FirstOrDefault();

                if (mapFile != null && File.Exists(mapFile))
                {
                    // An area whose tiles did not load is not an area. Caching it anyway is how a map
                    // made entirely of wall ends up in the world with nothing said about it.
                    if (!LoadMap(mapObj, mapFile, true))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(mapObj.ScriptKey))
                    {
                        mapObj.Scripts = ScriptManager.Load<AreaScript>(mapObj.ScriptKey, mapObj);
                    }

                    ServerContext.GlobalMapCache[mapObj.Id] = mapObj;
                }
            }
        }

        public Area Load(string name)
        {
            var path = Path.Combine(StoragePath, $"{name.ToLower()}.json");

            if (!File.Exists(path))
                return null;

            using var s = File.OpenRead(path);
            using var f = new StreamReader(s);
            var content = f.ReadToEnd();


            try
            {
                var obj = StorageManager.Deserialize<Area>(content);

                return obj;
            }
            catch (Exception ex)
            {
                ServerContext.Logger(ex.Message, Microsoft.Extensions.Logging.LogLevel.Error);
                ServerContext.Logger(ex.StackTrace, Microsoft.Extensions.Logging.LogLevel.Error);
                return null;
            }
        }

        public void Save(Area obj)
        {
            var path = Path.Combine(StoragePath, $"{obj.Name.ToLower()}.json");

            // The saved value must not depend on which platform wrote it. GetRelativePath answers with the
            // host separator, so the same map file was recorded one way on Windows and another here, and
            // every run rewrote the area files it had just read. Record one spelling everywhere.
            obj.FilePath = PathNetCore.GetRelativePath(".", ServerContext.StoragePath + "/maps/lod" + obj.Id + ".map")
                .Replace('\\', '/');

            var objString = StorageManager.Serialize(obj);
            File.WriteAllText(path, objString);
        }
    }
}