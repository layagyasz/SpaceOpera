using Cardamom.Mathematics.Geometry;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Common;

namespace SpaceOpera.View.StellarBodyViews
{
    public class StellarBodySubRegionBounds
    {
        public static SpaceSubRegionBounds ComputeBounds(StellarBodySubRegion region, float radius)
        {
            var edges = new SpaceSubRegionBounds.Edge[region.Neighbors!.Length];
            for (int i = 0; i < region.Neighbors.Length; ++i)
            {
                var left =
                    region.Center
                    + region.Neighbors[(i + region.Neighbors.Length - 1) % region.Neighbors.Length].Center
                    + region.Neighbors[i].Center;
                var right =
                    region.Center
                    + region.Neighbors[i].Center
                    + region.Neighbors[(i + 1) % region.Neighbors.Length].Center;
                edges[i] = new(new(radius * left.Normalized(), radius * right.Normalized()), -1, -1);
            }

            var c = region.Center.Normalized();
            return SpaceSubRegionBounds.FromEdges(radius * c, c, edges, Array.Empty<Line3?>());
        }
    }
}
