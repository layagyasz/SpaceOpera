using Cardamom.Mathematics.Geometry;
using OpenTK.Mathematics;
using SpaceOpera.View.Common;

namespace SpaceOpera.View.StarSystemViews
{
    public static class StarSystemSubRegionBounds
    {
        private static readonly float s_EdgeResolution = 0.02f * MathHelper.Pi;

        public static SpaceSubRegionBounds ComputeBounds(Vector3 center, float radius, float scale)
        {
            return SpaceSubRegionBounds.FromEdges(
                scale * center,
                Vector3.UnitY,
                Array.Empty<SpaceSubRegionBounds.Edge>(), 
                new Line3[]
                {
                    new(
                        Shape.GetCirclePoints(x => radius, s_EdgeResolution)
                            .Select(x => scale * (center + new Vector3(x.X, 0, x.Y)))
                            .ToArray(), 
                        Vector3.UnitY,
                        /* isLoop= */ true)
                });
        }
    }
}
