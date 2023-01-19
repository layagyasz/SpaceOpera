using Cardamom.Mathematics;

namespace SpaceOpera.Core.Universe.Generator
{
    class StellarBodyGeneratorOption
    {
        public StellarBodyGenerator? Generator { get; set; }
        public float Weight { get; set; }
        public Interval ThermalRange { get; set; }
        public Interval GravitationalRange { get; set; }

        public bool Satisfies(float temperature, float gravity)
        {
            return ThermalRange.Contains(temperature) && GravitationalRange.Contains(gravity);
        }
    }
}