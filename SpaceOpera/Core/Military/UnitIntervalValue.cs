namespace SpaceOpera.Core.Military
{
    public struct UnitIntervalValue
    {
        public static UnitIntervalValue Zero = new(0);

        private static readonly float s_Divisor = 100;

        public float RawValue { get; }
        public float UnitValue { get; }

        public UnitIntervalValue(float RawValue)
        {
            this.RawValue = RawValue;
            this.UnitValue = ToUnitInterval(RawValue);
        }

        public static float ToUnitInterval(float RawValue)
        {
            return RawValue / (RawValue + s_Divisor);
        }
        public override string ToString()
        {
            return string.Format("{0:N0} ({1:P0})", RawValue, UnitValue);
        }
    }
}