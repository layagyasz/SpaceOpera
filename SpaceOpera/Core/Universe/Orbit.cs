using MathNet.Numerics;
using OpenTK.Mathematics;

namespace SpaceOpera.Core.Universe
{
    public class Orbit
    {
        public Star Focus { get; }
        public float MajorAxis { get; }
        public float MinorAxis { get; }
        public float Eccentricity { get; }
        public float TimeOffset { get; }

        public Orbit(Star focus, float majorAxis, float eccentricity, float timeOffset)
        {
            Focus = focus;
            MajorAxis = majorAxis;
            MinorAxis = majorAxis * MathF.Sqrt(eccentricity * eccentricity + 1);
            Eccentricity = eccentricity;
            TimeOffset = timeOffset;
        }

        public float GetAverageDistance()
        {
            return 0.5f * MajorAxis * (1 + Eccentricity * Eccentricity / 2);
        }

        public float GetCircumference()
        {
            return 0.5f * (float)Math.PI * (MajorAxis + MinorAxis);
        }

        public Vector2 GetPoint(float angle)
        {
            var d = GetDistance(angle);
            return new Vector2(d * MathF.Cos(angle), d * MathF.Sin(angle));
        }

        public float GetDistance(float angle)
        {
            return 0.5f * MajorAxis * (1 - Eccentricity * Eccentricity) / (1 + Eccentricity * MathF.Cos(angle));
        }

        public float GetStellarTemperature()
        {
            return 3 * Focus.Temperature * MathF.Sqrt(Focus.Radius) 
                / (4 * MathF.Sqrt(GetAverageDistance() / Constants.AstralUnit));
        }
    }
}