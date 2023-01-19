namespace SpaceOpera.Core.Universe.Generator
{
    public class BiomeCondition
    {
        public Expression.ParameterName ParameterName { get; set; }
        public float Minimum { get; set; }
        public float Maximum { get; set; }

        public bool Satisfies(float[] parameters)
        {
            float p = Math.Min(Math.Max(parameters[(int)ParameterName], 0), 1);
            return p >= Minimum && p <= Maximum;
        }
    }
}