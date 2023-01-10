using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera
{
    public class WeightedVector<T> : ICollection<KeyValuePair<T, double>>
    {
        double[] _Values;
        T[] _Keys;
        double _Total;

        int _Alloc;

        public double Total
        {
            get
            {
                return _Total;
            }
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; } = false;

        public WeightedVector()
        {
            Clear();
        }

        public WeightedVector(WeightedVector<T> Copy)
        {
            this.Count = Copy.Count;
            this._Alloc = Copy._Alloc;
            this._Values = new double[_Alloc];
            this._Keys = new T[_Alloc];
            this._Total = Copy._Total;
            for (int i = 0; i < _Alloc; i++)
            {
                this._Keys[i] = Copy._Keys[i];
                this._Values[i] = Copy._Values[i];
            }
        }

        public IEnumerator<KeyValuePair<T, double>> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return new KeyValuePair<T, double>(_Keys[i], _Values[i]);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(double Weight, T Value)
        {
            if (Count + 1 == _Alloc)
            {
                _Alloc *= 2;
                T[] newK = new T[_Alloc];
                double[] newA = new double[_Alloc];
                Array.Copy(_Keys, newK, _Keys.Length);
                Array.Copy(_Values, newA, _Keys.Length);
                _Values = newA;
                _Keys = newK;
            }
            _Total += Weight;
            _Keys[Count] = Value;
            _Values[Count + 1] = _Values[Count] + Weight;
            ++Count;
        }

        public void Add(KeyValuePair<T, double> Value)
        {
            Add(Value.Value, Value.Key);
        }

        private int IndexOf(double V)
        {
            if (Count == 0) throw new Exception("Index on Empty WeightVector<T>");
            if (Count == 1) return 0;
            int i = 0;
            int j = Count - 1;
            int c = (j + i) / 2;
            while (j - i > 1)
            {
                if (V > _Values[c])
                {
                    i = c;
                    c = (j + i) / 2;
                }
                else if (V < _Values[c])
                {
                    j = c;
                    c = (j + i) / 2;
                }
                else break;
            }
            if (j - i == 1 && _Values[c] > V) c--;
            else if (j - i == 1 && _Values[c + 1] < V) c++;
            return c;
        }

        public T this[double V]
        {
            get
            {
                return _Keys[IndexOf(V * _Total)];
            }
            set
            {
                _Keys[IndexOf(V * _Total)] = value;
            }
        }

        public void Clear()
        {
            Count = 0;
            _Alloc = 1;
            _Values = new double[1];
            _Keys = new T[1];
        }

        public bool Contains(KeyValuePair<T, double> Value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<T, double> Value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<T, double>[] Values, int Index)
        {
            throw new NotImplementedException();
        }
    }
}