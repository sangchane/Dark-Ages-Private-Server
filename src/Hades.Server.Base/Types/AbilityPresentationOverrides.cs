using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Darkages.Network;
using Darkages.Network.ServerFormats;
using Newtonsoft.Json.Linq;

namespace Darkages.Types
{
    /// <summary>
    /// 운영 화면에서 고른 기술·마법의 이펙트·속도·사운드를 패킷 직전에 바꾼다.
    /// 피해·마나·쿨다운 같은 게임 규칙은 건드리지 않는다.
    /// </summary>
    public static class AbilityPresentationOverrides
    {
        private sealed class Values
        {
            public ushort? Effect { get; init; }
            public ushort? Speed { get; init; }
            public byte? Sound { get; init; }
        }

        private sealed class Scope : IDisposable
        {
            private readonly string _before;
            private bool _disposed;

            public Scope(string key)
            {
                _before = Current.Value;
                Current.Value = key;
            }

            public void Dispose()
            {
                if (_disposed) return;
                Current.Value = _before;
                _disposed = true;
            }
        }

        private static readonly AsyncLocal<string> Current = new AsyncLocal<string>();
        private static readonly object Sync = new object();
        private static Dictionary<string, Values> _values = new Dictionary<string, Values>(StringComparer.Ordinal);
        private static string _loadedPath;
        private static DateTime _loadedAt;

        public static IDisposable Begin(string kind, string name) =>
            new Scope(string.IsNullOrWhiteSpace(kind) || string.IsNullOrWhiteSpace(name)
                ? string.Empty
                : $"{kind.Trim().ToLowerInvariant()}:{name.Trim()}");

        /// <summary>시험과 운영 도구가 파일을 쓴 직후 즉시 다시 읽게 한다.</summary>
        public static void ReloadNow()
        {
            lock (Sync)
            {
                _loadedPath = null;
                _loadedAt = DateTime.MinValue;
                Load();
            }
        }

        public static void Apply(NetworkFormat format)
        {
            if (format == null || string.IsNullOrEmpty(Current.Value)) return;
            EnsureLoaded();
            if (!_values.TryGetValue(Current.Value, out var value)) return;

            if (format is ServerFormat29 effect)
            {
                if (value.Effect.HasValue)
                {
                    var number = value.Effect.Value;
                    if (effect.CasterEffect != 0) effect.CasterEffect = number;
                    if (effect.TargetEffect != 0) effect.TargetEffect = number;
                    if (effect.CasterEffect == 0 && effect.TargetEffect == 0) effect.TargetEffect = number;
                }

                if (value.Speed.HasValue) effect.Speed = value.Speed.Value;
            }
            else if (format is ServerFormat13 health && value.Sound.HasValue)
            {
                health.Sound = value.Sound.Value;
            }
            else if (format is ServerFormat19 sound && value.Sound.HasValue)
            {
                sound.Number = value.Sound.Value;
            }
        }

        private static void EnsureLoaded()
        {
            var path = Environment.GetEnvironmentVariable("LOD_ABILITY_OVERRIDES");
            var changed = !string.Equals(path, _loadedPath, StringComparison.Ordinal)
                          || (!string.IsNullOrWhiteSpace(path) && File.Exists(path)
                              && File.GetLastWriteTimeUtc(path) != _loadedAt)
                          || (!string.IsNullOrWhiteSpace(_loadedPath) && !File.Exists(_loadedPath));
            if (!changed) return;
            lock (Sync)
            {
                path = Environment.GetEnvironmentVariable("LOD_ABILITY_OVERRIDES");
                if (!string.Equals(path, _loadedPath, StringComparison.Ordinal)
                    || (!string.IsNullOrWhiteSpace(path) && File.Exists(path)
                        && File.GetLastWriteTimeUtc(path) != _loadedAt)
                    || (!string.IsNullOrWhiteSpace(_loadedPath) && !File.Exists(_loadedPath)))
                    Load();
            }
        }

        private static void Load()
        {
            var path = Environment.GetEnvironmentVariable("LOD_ABILITY_OVERRIDES");
            var loaded = new Dictionary<string, Values>(StringComparer.Ordinal);
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                try
                {
                    var root = JObject.Parse(File.ReadAllText(path));
                    if (root["abilities"] is JObject abilities)
                    {
                        foreach (var pair in abilities)
                        {
                            if (!(pair.Value is JObject entry) || !ValidKey(pair.Key)) continue;
                            loaded[pair.Key] = new Values
                            {
                                Effect = Number(entry["effect"], 1, 999),
                                Speed = Number(entry["speed"], 1, 255),
                                Sound = Byte(entry["sound"], 0, 255),
                            };
                        }
                    }
                }
                catch
                {
                    // 운영 파일은 원자적으로 쓰지만 사람이 깨뜨릴 수도 있다. 잘못된 값을 게임에 남기지 않는다.
                    loaded.Clear();
                }
            }

            _values = loaded;
            _loadedPath = path;
            _loadedAt = !string.IsNullOrWhiteSpace(path) && File.Exists(path)
                ? File.GetLastWriteTimeUtc(path)
                : DateTime.MinValue;
        }

        private static bool ValidKey(string key) =>
            !string.IsNullOrWhiteSpace(key) && (key.StartsWith("skill:", StringComparison.Ordinal)
                                                || key.StartsWith("spell:", StringComparison.Ordinal));

        private static ushort? Number(JToken token, int low, int high) =>
            token?.Type == JTokenType.Integer && token.Value<int>() >= low && token.Value<int>() <= high
                ? (ushort?) token.Value<int>()
                : null;

        private static byte? Byte(JToken token, int low, int high) =>
            token?.Type == JTokenType.Integer && token.Value<int>() >= low && token.Value<int>() <= high
                ? (byte?) token.Value<int>()
                : null;
    }
}
