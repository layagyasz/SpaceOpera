using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera
{
    struct CompositeKey<T, K>
    {
        public T Key1 { get; }
        public K Key2 { get; }

        private CompositeKey(T Key1, K Key2)
        {
            this.Key1 = Key1;
            this.Key2 = Key2;
        }

        public static CompositeKey<T, K> Create(T Key1, K Key2)
        {
            return new CompositeKey<T, K>(Key1, Key2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is CompositeKey<T, K> other)
            {
                return (Key1?.Equals(other.Key1) ?? other.Key1 == null) 
                    && (Key2?.Equals(other.Key2) ?? other.Key2 == null);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Key1.GetHashCode() * 31 + Key2.GetHashCode();
        }
    }
}