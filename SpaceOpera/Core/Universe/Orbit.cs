using Cardamom.Mathematics.Coordinates;
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

        public Vector2d GetPoint(double angle)
        {
            var d = GetDistance(angle);
            return new Vector2d(d * Math.Cos(angle), d * Math.Sin(angle));
        }

        public double GetDistance(double angle)
        {
            return 0.5f * MajorAxis * (1 - Eccentricity * Eccentricity) / (1 + Eccentricity * Math.Cos(angle));
        }

        public Polar2 GetPosition(double angle)
        {
            return new((float)GetDistance(angle), (float)angle);
        }

        public double GetProgression(double angle, float precision, int accuracy)
        {
            double e = angle;
            double f = e - Eccentricity * Math.Sin(angle) - angle;

            int i = 0;
            while ((Math.Abs(f) > precision) && (i < accuracy))
            {
                e -= f / (1f - Eccentricity * Math.Cos(e));
                f = e - Eccentricity * Math.Sin(e) - angle;
                ++i;
            }

            double sin = Math.Sin(e);
            double cos = Math.Cos(e);
            double fak = Math.Sqrt(1f - Eccentricity * Eccentricity);
            return Math.Atan2(fak * sin, cos - Eccentricity);
        }

        public float GetDayLengthInSeconds()
        {
            // TODO: Randomly generate
            return 60 * 60 * 24;
        }

        public float GetYearLengthInSeconds()
        {
            return 2 * MathHelper.Pi * MathF.Sqrt(
                MathF.Pow(1000 * MajorAxis / (2 * Constants.AstralUnit), 3) 
                / (Constants.GravitationalConstant * Focus.Mass));
        }

        public float GetStellarTemperature()
        {
            return 3 * Focus.Temperature * MathF.Sqrt(Focus.Radius) 
                / (4 * MathF.Sqrt(GetAverageDistance() / Constants.AstralUnit));
        }
    }
}