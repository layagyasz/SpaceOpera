using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera
{
    public class EnumMap<T, K> : IDictionary<T, K> where T : struct, IConvertible
    {
        readonly K[] _Values;

        public K this[T Key]
        {
            get => _Values[(int)(object)Key];
            set => _Values[(int)(object)Key] = value;
        }

        public ICollection<T> Keys
        {
            get
            {
                return Enum.GetValues(typeof(T)).Cast<T>().Where(this.ContainsKey).ToList();
            }
        }
        public ICollection<K> Values { get { return _Values; } }
        public int Count { get { return _Values.Length; } }
        public bool IsReadOnly { get; } = false;

        public EnumMap()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException(string.Format("Type {0} is not an enum.", typeof(T).Name));
            }
            _Values = new K[Enum.GetValues(typeof(T)).Length];
        }

        public EnumMap(EnumMap<T, K> Copy) : this()
        {
            _Values = Copy._Values.ToArray();
        }

        public IEnumerator<KeyValuePair<T, K>> GetEnumerator()
        {
            foreach (T key in Enum.GetValues(typeof(T)))
            {
                if (!this[key]?.Equals(default(K)) ?? false)
                {
                    yield return new KeyValuePair<T, K>(key, this[key]);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Clear()
        {
            for (int i = 0; i < _Values.Length; ++i)
            {
                _Values[i] = default;
            }
        }

        public bool ContainsKey(T Key)
        {
            return !this[Key]?.Equals(default(K)) ?? false;
        }

        public bool Contains(KeyValuePair<T, K> Value)
        {
            return this[Value.Key].Equals(Value.Value);
        }

        public void Add(T Key, K Value)
        {
            this[Key] = Value;
        }

        public void Add(KeyValuePair<T, K> Value)
        {
            this[Value.Key] = Value.Value;
        }

        public bool Remove(T Key)
        {
            this[Key] = default;
            return true;
        }

        public bool Remove(KeyValuePair<T, K> Value)
        {
            if (Contains(Value))
            {
                return Remove(Value.Key);
            }
            return false;
        }

        public bool TryGetValue(T Key, out K Value)
        {
            Value = this[Key];
            return true;
        }

        public void CopyTo(KeyValuePair<T, K>[] Values, int Index)
        {
            int i = 0;
            foreach (T Key in Keys)
            {
                Values[i + Index] = new KeyValuePair<T, K>(Key, this[Key]);
            }
        }
    }
}