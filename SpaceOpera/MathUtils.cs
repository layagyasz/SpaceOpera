using Cardamom.Mathematics;
using OpenTK.Mathematics;

namespace SpaceOpera
{
    public static class MathUtils
    {
        public static float AngleDistance(float left, float right)
        {
            return (right + 2 * MathF.PI - left) % (2 * MathF.PI);
        }

        public static float ArcLength(Vector3 left, Vector3 right, float radius)
        {
            return radius * MathF.Acos(Vector3.Dot(left, right) / (left.Length * right.Length));
        }

        public static float ArcLength(Vector2 left, Vector2 right, float radius)
        {
            return radius * MathF.Acos(Vector2.Dot(left, right) / (left.Length * right.Length));
        }

        public static Vector2 Perpendicular(Vector2 vector)
        {
            return new(-vector.Y, vector.X);
        }

        public static Vector2 PerpendicularFrom(Vector2 center, Vector2 vector)
        {
            var option1 = new Vector2(-vector.Y, vector.X);
            var option2 = new Vector2(vector.Y, -vector.X);
            return (center + option1).LengthSquared > (center + option2).LengthSquared ? option2 : option1;
        }

        public static Vector2 ProjectOntoCircle(Vector2 point, Vector2 direction, float radius2)
        {
            return point
                + Quadratic.Solve(1, 2 * Vector2.Dot(point, direction), point.LengthSquared - radius2).Item1!.Value
                * direction;
        }

        public static float StandardDeviation(IEnumerable<float> values)
        {
            return StandardDeviation(values, values.Average());
        }

        public static float StandardDeviation(IEnumerable<float> values, float mean)
        {
            return MathF.Sqrt(values.Sum(x => (x - mean) * (x - mean)));
        }

        public static float Theta(Vector2 left, Vector2 right)
        {
            return MathF.Atan2(right.Y - left.Y, right.X - left.X);
        }

        public static float TweenAngleBackward(float left, float right)
        {
            var a1 = ((left + right) / 2) % (2 * MathF.PI);
            var a2 = ((left + right) / 2 + MathF.PI) % (2 * MathF.PI);
            return AngleDistance(left, a1) > AngleDistance(left, a2) ? a1 : a2;
        }

        public static float TweenAngleForward(float left, float right)
        {
            var a1 = ((left + right) / 2) % (2 * MathF.PI);
            var a2 = ((left + right) / 2 + MathF.PI) % (2 * MathF.PI);
            return AngleDistance(left, a1) < AngleDistance(left, a2) ? a1 : a2;
        }
    }
}