using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera
{
    class EnumSet<T> : ISet<T>
    {
        readonly bool[] _Values;

        public int Count { get => _Values.Count(x => x); }
        public bool IsReadOnly { get; } = false;

        public EnumSet()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException(string.Format("Type {0} is not an enum.", typeof(T).Name));
            }
            _Values = new bool[Enum.GetValues(typeof(T)).Length];
        }

        public EnumSet(IEnumerable<T> Items)
            : this()
        {
            UnionWith(Items);
        }

        public EnumSet(params T[] Items)
            : this(Items.ToList()) { }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (Contains(item))
                {
                    yield return item;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool Add(T Item)
        {
            if (Contains(Item))
            {
                return false;
            }
            _Values[(int)(object)Item] = true;
            return true;
        }

        void ICollection<T>.Add(T Item)
        {
            Add(Item);
        }

        public void Clear()
        {
            for (int i=0;i<_Values.Length;++i)
            {
                _Values[i] = false;
            }
        }

        public bool Contains(T Item)
        {
            return _Values[(int)(object)Item];
        }

        public void CopyTo(T[] Array, int ArrayIndex)
        {
            foreach (var item in this)
            {
                Array[ArrayIndex++] = item;
            }
        }

        public void ExceptWith(IEnumerable<T> Other)
        {
            foreach (var item in Other)
            {
                Remove(item);
            }
        }

        public void IntersectWith(IEnumerable<T> Other)
        {
            var otherSet = new EnumSet<T>(Other);
            for (int i=0; i<_Values.Length; ++i)
            {
                _Values[i] &= otherSet._Values[i];
            }
        }

        public bool IsProperSubsetOf(IEnumerable<T> Other)
        {
            return IsSubsetOf(Other) && Count != Other.Count();
        }

        public bool IsProperSupersetOf(IEnumerable<T> Other)
        {
            return IsSupersetOf(Other) && Count != Other.Count();
        }

        public bool IsSubsetOf(IEnumerable<T> Other)
        {
            foreach (var item in this)
            {
                if (!Other.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsSupersetOf(IEnumerable<T> Other)
        {
            foreach (var item in Other)
            {
                if (!Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        public bool Overlaps(IEnumerable<T> Other)
        {
            return Other.Any(this.Contains);
        }

        public bool Remove(T Item)
        {
            if (!Contains(Item))
            {
                return false;
            }
            _Values[(int)(object)Item] = false;
            return true;
        }

        public bool SetEquals(IEnumerable<T> Other)
        {
            return IsSupersetOf(Other) && Count == Other.Count();
        }

        public void SymmetricExceptWith(IEnumerable<T> Other)
        {
            foreach (var item in Other)
            {
                _Values[(int)(object)item] ^= true;
            }
        }

        public void UnionWith(IEnumerable<T> Other)
        {
            foreach (var item in Other)
            {
                Add(item);
            }
        }
    }
}