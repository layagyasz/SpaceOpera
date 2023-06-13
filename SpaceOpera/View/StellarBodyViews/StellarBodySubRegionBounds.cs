using Cardamom.Mathematics.Geometry;
using OpenTK.Mathematics;
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
                var left = ComputeCircumcenterDeterministic(
                                region,
                                (i + region.Neighbors.Length - 1) % region.Neighbors.Length).Normalized();
                var right = ComputeCircumcenterDeterministic(region, i).Normalized();
                edges[i] = new(new(radius * left, radius * right), new(-left, -right), -1, -1);
            }

            return SpaceSubRegionBounds.FromEdges(radius * region.Center, region.Center, edges, Array.Empty<Line3?>());
        }

        private static Vector3 ComputeCircumcenterDeterministic(StellarBodySubRegion region, int index)
        {
            var a = region.Neighbors![index];
            var b = region.Neighbors[(index + 1) % region.Neighbors.Length];
            if (a.Id < b.Id && a.Id < region.Id)
            {
                int i = Array.IndexOf(a.Neighbors!, region);
                return ComputeCircumcenter(a, (i + a.Neighbors!.Length - 1) % a.Neighbors.Length);
            }
            if (b.Id < a.Id && b.Id < region.Id)
            {
                int i = Array.IndexOf(b.Neighbors!, region);
                return ComputeCircumcenter(b, i);
            }
            return ComputeCircumcenter(region, index);
        }

        private static Vector3 ComputeCircumcenter(StellarBodySubRegion region, int index)
        {
            var a = region.Neighbors![index].Center;
            var b = region.Neighbors[(index + 1) % region.Neighbors.Length].Center;
            var ha = 0.5f * (a + region.Center);
            var hb = 0.5f * (b + region.Center);
            var hap = Vector3.Cross(ha - region.Center, region.Center);
            var hbp = Vector3.Cross(hb - region.Center, region.Center);
            return ha + hap * (float)GetRayIntersection(new(ha, hap), new(hb, hbp));
        }

        private static float GetRayIntersection(Ray3 left, Ray3 right)
        {
            var q = new Vector3(right.Point - left.Point);
            var o = Vector3.Cross(left.Direction, right.Direction);
            return Vector3.Dot(Vector3.Cross(q, right.Direction), o) / o.LengthSquared;
        }
    }
}
