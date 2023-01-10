using Cardamom.Spatial;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera
{
    class MathUtils
    {
        public static double AngleDistance(double Left, double Right)
        {
            return (Right + 2 * Math.PI - Left) % (2 * Math.PI);
        }

        public static double ArcLength(Vector4f Left, Vector4f Right, double Radius)
        {
            return Radius
                * Math.Acos((Left.X * Right.X + Left.Y * Right.Y + Left.Z * Right.Z) 
                    / (Magnitude(Left) * Magnitude(Right)));
        }

        public static double ArcLength(Vector2f Left, Vector2f Right, double Radius)
        {
            return Radius * Math.Acos((Left.X * Right.X + Left.Y * Right.Y) / (Magnitude(Left) * Magnitude(Right)));
        }
        public static double Clamp(double Value, double Min, double Max)
        {
            return Math.Min(Max, Math.Max(Value, Min));
        }

        public static float Distance(Vector2f Left, Vector2f Right)
        {
            return Magnitude(Left - Right);
        }

        public static float Magnitude(Vector4f Vector)
        {
            return (float)Math.Sqrt(Magnitude2(Vector));
        }

        public static float Magnitude(Vector2f Vector)
        {
            return (float)Math.Sqrt(Magnitude2(Vector));
        }

        public static float Magnitude2(Vector4f Vector)
        {
            return Vector.X * Vector.X + Vector.Y * Vector.Y + Vector.Z * Vector.Z;
        }

        public static float Magnitude2(Vector2f Vector)
        {
            return Vector.X * Vector.X + Vector.Y * Vector.Y;
        }

        public static Vector2f Normalize(Vector2f Vector)
        {
            return Vector / Magnitude(Vector);
        }

        public static Vector2f Perpendicular(Vector2f Vector)
        {
            return new Vector2f(-Vector.Y, Vector.X);
        }

        public static Vector2f PerpendicularFrom(Vector2f Initial, Vector2f Vector)
        {
            var option1 = new Vector2f(-Vector.Y, Vector.X);
            var option2 = new Vector2f(Vector.Y, -Vector.X);
            return Magnitude2(Initial + option1) > Magnitude2(Initial + option2) ? option2 : option1;
        }

        public static Vector2f ProjectOntoCircle(Vector2f Initial, Vector2f UnitVector, float Radius2)
        {
            return Initial
                + Quadratic(
                    1, 2 * (Initial.X * UnitVector.X + Initial.Y * UnitVector.Y), Magnitude2(Initial) - Radius2)
                * UnitVector;
        }

        public static float Quadratic(float A, float B, float C)
        {
            return (-B + (float)Math.Sqrt(B * B - 4 * A * C)) / (2 * A);
        }

        public static float StandardDeviation(IEnumerable<float> Values)
        {
            return StandardDeviation(Values, Values.Average());
        }

        public static float StandardDeviation(IEnumerable<float> Values, float Mean)
        {
            return (float)Math.Sqrt(Values.Sum(x => (x - Mean) * (x - Mean)));
        }

        public static double Theta(Vector2f Left, Vector2f Right)
        {
            return Math.Atan2(Right.Y - Left.Y, Right.X - Left.X);
        }

        public static double TweenAngleBackward(double Left, double Right)
        {
            var a1 = ((Left + Right) / 2) % (2 * Math.PI);
            var a2 = ((Left + Right) / 2 + Math.PI) % (2 * Math.PI);
            return AngleDistance(Left, a1) > AngleDistance(Left, a2) ? a1 : a2;
        }

        public static double TweenAngleForward(double Left, double Right)
        {
            var a1 = ((Left + Right) / 2) % (2 * Math.PI);
            var a2 = ((Left + Right) / 2 + Math.PI) % (2 * Math.PI);
            return AngleDistance(Left, a1) < AngleDistance(Left, a2) ? a1 : a2;
        }
    }
}