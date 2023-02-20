using Cardamom.Mathematics.Coordinates;
using OpenTK.Mathematics;

namespace SpaceOpera.View
{
    public static class Shape
    {
        public static Vector2[] GetArcPoints(float start, float end, Func<float, float> radiusFn, float resolution)
        {
            var points = new List<Vector2>
            {
                radiusFn(start) * new Vector2(MathF.Cos(start), MathF.Sin(start))
            };
            for (
                float i = start + resolution;
                MathUtils.AngleDistance(start, end) > MathUtils.AngleDistance(start, i);
                i += resolution)
            {
                points.Add(radiusFn(i) * new Vector2(MathF.Cos(i), MathF.Sin(i)));
            }
            points.Add(radiusFn(end) * new Vector2(MathF.Cos(end), MathF.Sin(end)));
            return points.ToArray();
        }

        public static Vector2[] GetCirclePoints(Func<float, float> RadiusFn, float Resolution)
        {
            var points = new Vector2[(int)(2 * Math.PI / Resolution) + 2];
            for (int i = 0; i < points.Length - 1; ++i)
            {
                float angle = Resolution * i;
                points[i] = RadiusFn(angle) * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            }
            points[points.Length - 1] = points[0];
            return points;
        }
    }
}
