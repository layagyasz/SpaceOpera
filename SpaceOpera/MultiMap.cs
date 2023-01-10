using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera
{
    class MultiMap<TKey, TValue> : IDictionary<TKey, IEnumerable<TValue>>
    {
        private readonly Dictionary<TKey, List<TValue>> _Dictionary = new Dictionary<TKey, List<TValue>>();

        public IEnumerable<TValue> this[TKey Key]
        {
            get => _Dictionary.ContainsKey(Key) ? _Dictionary[Key] : Enumerable.Empty<TValue>();
            set => _Dictionary[Key] = value.ToList();
        }

        public ICollection<TKey> Keys => _Dictionary.Keys;

        public ICollection<IEnumerable<TValue>> Values => _Dictionary.Values.Select(x => x.AsEnumerable()).ToList();

        public int Count => _Dictionary.Count;
        public bool IsReadOnly => false;

        public MultiMap() { }

        public MultiMap(IDictionary<TKey, IEnumerable<TValue>> Copy)
        {
            foreach (var entry in Copy)
            {
                Add(entry);
            }
        }

        public void Add(TKey Key, TValue Value)
        {
            if (_Dictionary.ContainsKey(Key))
            {
                _Dictionary[Key].Add(Value);
            }
            else
            {
                _Dictionary.Add(Key, new List<TValue>() { Value });
            }
        }

        public void Add(TKey Key, IEnumerable<TValue> Value)
        {
            if (_Dictionary.ContainsKey(Key))
            {
                _Dictionary[Key].AddRange(Value);
            }
            else
            {
                _Dictionary.Add(Key, Value.ToList());
            }
        }

        public void Add(KeyValuePair<TKey, IEnumerable<TValue>> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, IEnumerable<TValue>> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key)
        {
            return _Dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, IEnumerable<TValue>>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, IEnumerable<TValue>>> GetEnumerator()
        {
            return _Dictionary.Select(
                x => new KeyValuePair<TKey, IEnumerable<TValue>>(x.Key, x.Value)).GetEnumerator();
        }

        public bool Remove(TKey Key)
        {
            return _Dictionary.Remove(Key);
        }

        public bool Remove(TKey Key, TValue Value)
        {
            if (_Dictionary.ContainsKey(Key))
            {
                return _Dictionary[Key].Remove(Value);
            }
            return false;
        }

        public int RemoveAll(TKey Key, Predicate<TValue> Predicate)
        {
            if (_Dictionary.ContainsKey(Key))
            {
                return _Dictionary[Key].RemoveAll(Predicate);
            }
            return 0;
        }

        public bool Remove(KeyValuePair<TKey, IEnumerable<TValue>> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out IEnumerable<TValue> value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}