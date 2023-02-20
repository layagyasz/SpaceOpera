using Cardamom.Mathematics.Geometry;
using OpenTK.Mathematics;
using SpaceOpera.View.Common;

namespace SpaceOpera.View.StarSystemViews
{
    public static class OrbitBounds
    {
        private static readonly float s_EdgeResolution = 0.02f * MathHelper.Pi;

        public static SpaceSubRegionBounds ComputeBounds(float y, float radius, float scale)
        {
            return SpaceSubRegionBounds.FromEdges(
                new(),
                Vector3.UnitY,
                Array.Empty<SpaceSubRegionBounds.Edge>(), 
                new Line3[]
                {
                    new(
                        Shape.GetCirclePoints(x => scale * radius, s_EdgeResolution)
                            .Select(x => new Vector3(x.X, y, x.Y))
                            .ToArray(), 
                        /* isLoop= */ true)
                });
        }
    }
}
