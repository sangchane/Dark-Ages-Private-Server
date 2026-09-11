#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Darkages.Types;


#endregion

namespace Darkages.Network.Object
{
    public sealed class ObjectService
    {
        private readonly Dictionary<int, IDictionary<Type, object>> _spriteCollections =
            new Dictionary<int, IDictionary<Type, object>>();

        public ObjectService()
        {
            foreach (var map in ServerContext.GlobalMapCache.Values)
                _spriteCollections.Add(map.Id, new Dictionary<Type, object>
                {
                    {typeof(Monster), new SpriteList<Monster>(Enumerable.Empty<Monster>())},
                    {typeof(Aisling), new SpriteList<Aisling>(Enumerable.Empty<Aisling>())},
                    {typeof(Mundane), new SpriteList<Mundane>(Enumerable.Empty<Mundane>())},
                    {typeof(Item), new SpriteList<Item>(Enumerable.Empty<Item>())},
                    {typeof(Money), new SpriteList<Money>(Enumerable.Empty<Money>())}
                });
        }

        public void AddGameObject<T>(T obj) where T : Sprite
        {
            if (obj.XPos >= byte.MaxValue)
                return;

            if (obj.YPos >= byte.MaxValue)
                return;

            if (!_spriteCollections.ContainsKey(obj.CurrentMapId))
                return;

            if (_spriteCollections.ContainsKey(obj.CurrentMapId))
            {
                if (_spriteCollections[obj.CurrentMapId].ContainsKey(typeof(T)))
                {
                    var objCollection = (SpriteList<T>) _spriteCollections[obj.CurrentMapId][typeof(T)];
                    objCollection.Add(obj);
                }
            }
        }

        public T Query<T>(Area map, Predicate<T> predicate) where T : Sprite
        {
            if (map == null)
            {
                var values = _spriteCollections.Select(i => (SpriteList<T>) i.Value[typeof(T)]);

                foreach (var obj in values)
                    if (obj.Any())
                        return obj.Query(predicate);
            }
            else
            {
                if (_spriteCollections.ContainsKey(map.Id))
                {
                    var obj = (SpriteList<T>) _spriteCollections[map.Id][typeof(T)];
                    var queryResult = obj.Query(predicate);
                    {
                        return queryResult;
                    }
                }
            }

            return null;
        }

        public IEnumerable<T> QueryAll<T>(Area map, Predicate<T> predicate) where T : Sprite
        {
            if (map == null)
            {
                var values = _spriteCollections.Select(i => (SpriteList<T>) i.Value[typeof(T)]);
                var stack = new List<T>();

                foreach (var obj in values)
                    if (obj.Any())
                        stack.AddRange(obj.QueryAll(predicate));

                return stack;
            }

            {
                var obj = (SpriteList<T>) _spriteCollections[map.Id][typeof(T)];
                var queryResult = obj.QueryAll(predicate);
                {
                    return queryResult;
                }
            }
        }

        public void RemoveAllGameObjects<T>(T[] objects) where T : Sprite
        {
            if (objects == null)
                return;

            for (uint i = 0; i < objects.Length; i++)
                RemoveGameObject(objects[i]);
        }

        public void RemoveGameObject<T>(T obj) where T : Sprite
        {
            if (obj != null && !_spriteCollections.ContainsKey(obj.CurrentMapId))
                return;

            if (obj != null)
            {
                var objCollection = (SpriteList<T>) _spriteCollections[obj.CurrentMapId][typeof(T)];
                objCollection.Delete(obj);
            }
        }
    }

    public class SpriteList<T> : IEnumerable<T>
        where T : Sprite
    {
        public readonly List<T> Values;

        public SpriteList(IEnumerable<T> values)
        {
            Values = new List<T>(values);
        }

        public bool Add(T obj)
        {
            lock (Values)
            {
                if (Values.Any(i => i.Serial == obj.Serial))
                {
                    var eobj = Values.FindIndex(idx => idx.Serial == obj.Serial);

                    if (eobj >= 0)
                    {
                        Values[eobj] = obj;
                        return false;
                    }
                }
                else
                {
                    Values.Add(obj);
                    return true;
                }
            }

            return false;
        }

        public void Delete(T obj)
        {
            // Add already takes this lock. Removing without it let a reader walk the list while items moved
            // under it, which is what threw IndexOutOfRange out of the update loop under ten connections.
            lock (Values)
            {
                for (var i = Values.Count - 1; i >= 0; i--)
                {
                    var subject = obj as Sprite;
                    var predicate = Values[i] as Sprite;

                    if (subject == predicate) Values.RemoveAt(i);
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        /// <summary>
        /// Reads run against a copy. Checking <c>Values.Count &gt; i</c> and then reading <c>Values[i]</c>
        /// is two steps, and another connection can remove an item between them — which is exactly what
        /// threw here. Copying under the lock costs one array per call and cannot race at all.
        /// </summary>
        public T Query(Predicate<T> predicate)
        {
            T[] snapshot;

            lock (Values)
            {
                snapshot = Values.ToArray();
            }

            for (var i = snapshot.Length - 1; i >= 0; i--)
            {
                var item = snapshot[i];

                if (item != null && predicate(item))
                {
                    return item.Abyss ? default : item;
                }
            }

            return default;
        }

        /// <summary>
        /// As <see cref="Query" />, and for one more reason: this hands results back one at a time, so the
        /// caller is still walking the list long after the call. A copy is the only thing that holds still.
        /// </summary>
        public IEnumerable<T> QueryAll(Predicate<T> predicate)
        {
            T[] snapshot;

            lock (Values)
            {
                snapshot = Values.ToArray();
            }

            for (var i = snapshot.Length - 1; i >= 0; i--)
            {
                var item = snapshot[i];

                if (item != null && predicate(item))
                {
                    yield return item.Abyss ? default : item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}