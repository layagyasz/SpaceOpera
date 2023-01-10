using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera
{
    class Optional<T>
    {
        public T Value { get; }
        public bool HasValue { get; }

        private Optional(T Value, bool HasValue)
        {
            this.Value = Value;
            this.HasValue = HasValue;
        }

        public static Optional<T> Of(T Value)
        {
            return new Optional<T>(Value, true);
        }

        public static Optional<T> Empty()
        {
            return new Optional<T>(default, false);
        }
    }
}