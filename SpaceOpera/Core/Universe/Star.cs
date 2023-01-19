namespace SpaceOpera.Core.Universe
{
    public class Star
    {
        private static readonly float s_BoltzmanConstant = 5.6704e-8f;

        public string Name { get; private set; }
        public float Temperature { get; }
        public float Radius { get; }
        public float Mass { get; }
        public float Luminosity { get; }

        public Star(float temperature, float radius, float mass)
        {
            Temperature = temperature;
            Radius = radius;
            Mass = mass;
            Luminosity = s_BoltzmanConstant * 4 * MathF.PI * MathF.Pow(radius, 2) * MathF.Pow(temperature, 4);
        }

        public void SetName(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return string.Format(
                "[Star: Name={0}, Temperature={1}, Radius={2}, Mass={3}, Luminosity={4}",
                Name, 
                Temperature,
                Radius,
                Mass, 
                Luminosity);
        }
    }
}