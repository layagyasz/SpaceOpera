namespace SpaceOpera.Core.Politics
{
    public class RandomValue
    {
        public List<string>? StaticString { get; set; }
        public int MinimumIntegerValue { get; set; }
        public int MaximumIntegerValue { get; set; }

        public object Generate(Random Random)
        {
            if (StaticString != null)
            {
                return StaticString[Random.Next(0, StaticString.Count)];
            }
            return Random.Next(MinimumIntegerValue, MaximumIntegerValue);
        }
    }
}
