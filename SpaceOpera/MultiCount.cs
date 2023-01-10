using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera
{
    class MultiCount<T> : IDictionary<T, int>
    {
        private readonly Dictionary<T, int> _Counts = new Dictionary<T, int>();

        public ICollection<T> Keys { get { return _Counts.Keys; } }
        public ICollection<int> Values { get { return _Counts.Values; } }
        public int Count { get { return _Counts.Count; } }
        public bool IsReadOnly { get; } = false;

        public int this[T Key]
        {
            get { return Get(Key); }
            set { Override(Key, value); }
        }

        public IEnumerable<Count<T>> GetCounts()
        {
            return this.Select(x => new Count<T>(x.Key, x.Value));
        }

        public int GetTotal()
        {
            return Values.Sum();
        }

        public void Add(T Key, int Amount)
        {
            if (_Counts.ContainsKey(Key))
            { 
                int newAmount = _Counts[Key] + Amount;
                if (newAmount != 0)
                {
                    _Counts[Key] += Amount;
                }
                else
                {
                    _Counts.Remove(Key);
                }
            }
            else
            {
                if (Amount != 0)
                {
                    _Counts.Add(Key, Amount);
                }
            }
        }

        public void Add(KeyValuePair<T, int> KeyValuePair)
        {
            Add(KeyValuePair.Key, KeyValuePair.Value);
        }

        public void Add(MultiCount<T> Counter)
        {
            foreach (var entry in Counter)
            {
                Add(entry);
            }
        }

        public void Clear()
        {
            _Counts.Clear();
        }

        public bool Contains(KeyValuePair<T, int> KeyValuePair)
        {
            return _Counts.Contains(KeyValuePair);
        }

        public bool ContainsKey(T Key)
        {
            return _Counts.ContainsKey(Key);
        }

        public MultiCount<T> Copy()
        {
            MultiCount<T> newCounter = new MultiCount<T>();
            foreach (var count in this)
            {
                newCounter.Add(count);
            }
            return newCounter;
        }

        public void CopyTo(KeyValuePair<T, int>[] KeyValuePairs, int Index)
        {
            throw new NotImplementedException();
        }

        public int Get(T Key)
        {
            bool found = _Counts.TryGetValue(Key, out int value);
            return found ? value : 0;
        }

        public IEnumerator<KeyValuePair<T, int>> GetEnumerator()
        {
            return _Counts.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Override(T Key, int Value)
        {
            if (Value == 0)
            {
                _Counts.Remove(Key);
            }
            else
            {
                if (_Counts.ContainsKey(Key))
                {
                    _Counts[Key] = Value;
                }
                else
                {
                    _Counts.Add(Key, Value);
                }
            }
        }

        public void Override(MultiCount<T> OverrideCounters)
        {
            foreach (var count in OverrideCounters)
            {
                Override(count.Key, count.Value);
            }
        }

        public bool Remove(T Key)
        {
            return _Counts.Remove(Key);
        }

        public bool Remove(KeyValuePair<T, int> KeyValuePair)
        {
            return _Counts.Remove(KeyValuePair.Key);
        }

        public bool TryGetValue(T Key, out int Value)
        {
            return _Counts.TryGetValue(Key, out Value);
        }

        public static MultiCount<T> operator +(MultiCount<T> A, MultiCount<T> B)
        {
            MultiCount<T> newCounter = A.Copy();
            foreach (var count in B)
            {
                newCounter.Add(count);
            }
            return newCounter;
        }

        public static MultiCount<T> operator -(MultiCount<T> A, MultiCount<T> B)
        {
            MultiCount<T> newCounter = A.Copy();
            foreach (var count in B)
            {
                newCounter.Add(count.Key, -count.Value);
            }
            return newCounter;
        }

        public static MultiCount<T> operator *(int Scale, MultiCount<T> A)
        {
            MultiCount<T> newCounter = new MultiCount<T>();
            foreach (var count in A)
            {
                newCounter.Add(count.Key, Scale * count.Value);
            }
            return newCounter;
        }
    }
}