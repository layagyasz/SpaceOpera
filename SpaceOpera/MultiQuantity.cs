using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera
{
    class MultiQuantity<T> : IDictionary<T, float>
    {
        private readonly Dictionary<T, float> _Quantities = new Dictionary<T, float>();

        public ICollection<T> Keys { get { return _Quantities.Keys; } }
        public ICollection<float> Values { get { return _Quantities.Values; } }
        public int Count { get { return _Quantities.Count; } }
        public bool IsReadOnly { get; } = false;

        public float this[T Key]
        {
            get { return Get(Key); }
            set { Override(Key, value); }
        }

        public IEnumerable<Quantity<T>> GetQuantities()
        {
            return this.Select(x => new Quantity<T>(x.Key, x.Value));
        }

        public float GetTotal()
        {
            return Values.Sum();
        }

        public void Add(T Key, float Amount)
        {
            if (_Quantities.ContainsKey(Key))
            {
                _Quantities[Key] += Amount;
            }
            else
            {
                _Quantities.Add(Key, Amount);
            }
        }

        public void Add(KeyValuePair<T, float> KeyValuePair)
        {
            Add(KeyValuePair.Key, KeyValuePair.Value);
        }

        public void Add(MultiQuantity<T> Counter)
        {
            foreach (var entry in Counter)
            {
                Add(entry);
            }
        }

        public void Clear()
        {
            _Quantities.Clear();
        }

        public bool Contains(KeyValuePair<T, float> KeyValuePair)
        {
            return _Quantities.Contains(KeyValuePair);
        }

        public bool ContainsKey(T Key)
        {
            return _Quantities.ContainsKey(Key);
        }

        public MultiQuantity<T> Copy()
        {
            MultiQuantity<T> newQuantity = new MultiQuantity<T>();
            foreach (var count in this)
            {
                newQuantity.Add(count);
            }
            return newQuantity;
        }

        public void CopyTo(KeyValuePair<T, float>[] KeyValuePairs, int Index)
        {
            throw new NotImplementedException();
        }

        public float Get(T Key)
        {
            bool found = _Quantities.TryGetValue(Key, out float value);
            return found ? value : 0;
        }

        public IEnumerator<KeyValuePair<T, float>> GetEnumerator()
        {
            return _Quantities.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Override(T Key, float Value)
        {
            if (_Quantities.ContainsKey(Key))
            {
                _Quantities[Key] = Value;
            }
            else
            {
                _Quantities.Add(Key, Value);
            }
        }

        public void Override(MultiQuantity<T> OverrideQuantities)
        {
            foreach (var count in OverrideQuantities)
            {
                Override(count.Key, count.Value);
            }
        }

        public bool Remove(T Key)
        {
            return _Quantities.Remove(Key);
        }

        public bool Remove(KeyValuePair<T, float> KeyValuePair)
        {
            return _Quantities.Remove(KeyValuePair.Key);
        }

        public bool TryGetValue(T Key, out float Value)
        {
            return _Quantities.TryGetValue(Key, out Value);
        }

        public static MultiQuantity<T> operator *(float C, MultiQuantity<T> A)
        {
            MultiQuantity<T> newQuantity = new MultiQuantity<T>();
            foreach (var quantity in A)
            {
                newQuantity.Add(quantity.Key, C * quantity.Value);
            }
            return newQuantity;
        }
    }
}