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
    }
}
