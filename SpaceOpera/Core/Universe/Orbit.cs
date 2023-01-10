using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe
{
    class Orbit
    {
        public Star Focus { get; }
        public float MajorAxis { get; }
        public float MinorAxis { get; }
        public float Eccentricity { get; }

        public Orbit(Star Focus, float MajorAxis, float Eccentricity)
        {
            this.Focus = Focus;
            this.MajorAxis = MajorAxis;
            this.MinorAxis = (float)(MajorAxis * Math.Sqrt(Eccentricity * Eccentricity + 1));
            this.Eccentricity = Eccentricity;
        }

        public float GetAverageDistance()
        {
            return 0.5f * MajorAxis * (1 + Eccentricity * Eccentricity / 2);
        }

        public float GetCircumference()
        {
            return 0.5f * (float)Math.PI * (MajorAxis + MinorAxis);
        }

        public Vector2f GetPoint(double Angle)
        {
            var d = GetDistance(Angle);
            return new Vector2f(d * (float)Math.Cos(Angle), d * (float)Math.Sin(Angle));
        }

        public float GetDistance(double Angle)
        {
            return (MajorAxis / 2) * (1 - Eccentricity * Eccentricity) / (1 + Eccentricity * (float)Math.Cos(Angle));
        }
    }
}