using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military
{
    struct UnitIntervalValue
    {
        public static UnitIntervalValue ZERO = new UnitIntervalValue(0);

        private static readonly float DIVISOR = 100;

        public float RawValue { get; }
        public float UnitValue { get; }

        public UnitIntervalValue(float RawValue)
        {
            this.RawValue = RawValue;
            this.UnitValue = ToUnitInterval(RawValue);
        }

        public static float ToUnitInterval(float RawValue)
        {
            return RawValue / (RawValue + DIVISOR);
        }
        public override string ToString()
        {
            return string.Format("{0:N0} ({1:P0})", RawValue, UnitValue);
        }
    }
}