namespace SpaceOpera.Core.Military
{
    public readonly struct UnitIntervalValue
    {
        public static readonly UnitIntervalValue Zero = new(0);

        private static readonly float s_Divisor = 100;

        public float RawValue { get; }
        public float UnitValue { get; }

        public UnitIntervalValue(float rawValue)
        {
            RawValue = rawValue;
            UnitValue = ToUnitInterval(rawValue);
        }

        public static float ToUnitInterval(float rawValue)
        {
            return rawValue / (rawValue + s_Divisor);
        }
        public override string ToString()
        {
            return string.Format("{0:N0} ({1:P0})", RawValue, UnitValue);
        }
    }
}