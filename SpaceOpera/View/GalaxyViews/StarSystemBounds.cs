using Cardamom.Graphing;
using Cardamom.Mathematics.Geometry;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.GalaxyViews
{
    public static class StarSystemBounds
    {
        private static readonly float s_RadiusBuffer = 1.05f;
        private static readonly float s_EdgeResolution = 0.05f * MathHelper.Pi;
        private static readonly int s_OutEdgeIndex = 0;

        public static SpaceSubRegionBounds ComputeBounds(StarSystem starSystem, float galaxyRadius)
        {
            galaxyRadius *= s_RadiusBuffer * s_RadiusBuffer;

            var outer = new float[1][];
            Line3[] outerEdges = new Line3[1];
            var edges = ComputeEdges(starSystem, galaxyRadius * galaxyRadius, outer);
            var outerThetas = outer[s_OutEdgeIndex];
            if (outerThetas[0] != outerThetas[1])
            {
                if (outerThetas[0] > outerThetas[1])
                {
                    outerThetas[1] += 2 * MathHelper.Pi;
                }
                var outerPoints = 
                    Shape.GetArcPoints(outerThetas[0], outerThetas[1], x => galaxyRadius, s_EdgeResolution);
                var a = new Vector3[outerPoints.Length];
                for (int i = 0; i < outerPoints.Length - 1; ++i)
                {
                    a[i] = ToVector3(outerPoints[i]);
                }
                outerEdges[s_OutEdgeIndex] = new(a);
            }

            return SpaceSubRegionBounds.FromEdges(starSystem.Position, edges, outerEdges);
        }

        private static SpaceSubRegionBounds.Edge[] ComputeEdges(
            StarSystem starSystem, float galaxyRadius2, float[][] outerEdges)
        {
            var edges = new SpaceSubRegionBounds.Edge[starSystem.Neighbors!.Count];
            for (int i = 0; i < starSystem.Neighbors.Count; ++i)
            {
                edges[i] = ComputeEdgeFor(starSystem, i, galaxyRadius2, outerEdges);
            }
            return edges;
        }

        private static SpaceSubRegionBounds.Edge ComputeEdgeFor(
            StarSystem starSystem, int neighborIndex, float galaxyRadius2, float[][] outerEdges)
        {
            var current = starSystem.Neighbors![neighborIndex];
            var left =
                starSystem.Neighbors[(neighborIndex + starSystem.Neighbors.Count - 1) % starSystem.Neighbors.Count];
            var right = starSystem.Neighbors[(neighborIndex + 1) % starSystem.Neighbors.Count];
            var leftCircumcenter = GetCircumcenter(starSystem, left, current);
            var rightCircumcenter = GetCircumcenter(starSystem, current, right);

            if (leftCircumcenter.HasValue && rightCircumcenter.HasValue)
            {
                bool leftInside = leftCircumcenter.Value.LengthSquared < galaxyRadius2;
                bool rightInside = rightCircumcenter.Value.LengthSquared < galaxyRadius2;
                if (leftInside && rightInside)
                {
                    return new(new(ToVector3(leftCircumcenter.Value), ToVector3(rightCircumcenter.Value)), -1, -1);
                }
                if (leftInside && !rightInside)
                {
                    var outerPoint =
                        MathUtils.ProjectOntoCircle(
                            leftCircumcenter.Value,
                            (rightCircumcenter.Value - leftCircumcenter.Value).Normalized(),
                            galaxyRadius2);
                    outerEdges[s_OutEdgeIndex][0] = MathF.Atan2(outerPoint.Y, outerPoint.X);
                    return new(new(ToVector3(leftCircumcenter.Value), ToVector3(outerPoint)), s_OutEdgeIndex, -1);
                }
                if (!leftInside && rightInside)
                {
                    var outerPoint =
                        MathUtils.ProjectOntoCircle(
                            rightCircumcenter.Value,
                            (leftCircumcenter.Value - rightCircumcenter.Value).Normalized(),
                            galaxyRadius2);
                    outerEdges[s_OutEdgeIndex][1] = MathF.Atan2(outerPoint.Y, outerPoint.X);
                    return new(new(ToVector3(outerPoint), ToVector3(rightCircumcenter.Value)), -1, s_OutEdgeIndex);
                }
                return new(null, -1, -1);
            }
            if (leftCircumcenter.HasValue && !rightCircumcenter.HasValue)
            {
                bool leftInside = leftCircumcenter.Value.LengthSquared < galaxyRadius2;
                if (leftInside)
                {
                    var mean = .5f * (starSystem.Position + current.Position);
                    var outerPoint =
                        MathUtils.ProjectOntoCircle(
                            mean.Xz,
                            -MathUtils.PerpendicularFrom(
                                mean.Xz, current.Position.Xz - starSystem.Position.Xz).Normalized(),
                            galaxyRadius2);
                    outerEdges[s_OutEdgeIndex][0] = MathF.Atan2(outerPoint.Y, outerPoint.X);
                    return new(new(ToVector3(leftCircumcenter.Value), ToVector3(outerPoint)), s_OutEdgeIndex, -1);
                }
                return new(null, -1, -1);
            }
            if (rightCircumcenter.HasValue && !leftCircumcenter.HasValue)
            {
                bool rightInside = rightCircumcenter.Value.LengthSquared < galaxyRadius2;
                if (rightInside)
                {
                    var mean = .5f * (starSystem.Position + current.Position);
                    var outerPoint =
                        MathUtils.ProjectOntoCircle(
                            mean.Xz,
                            -MathUtils.PerpendicularFrom(
                                mean.Xz, current.Position.Xz - starSystem.Position.Xz).Normalized(),
                            galaxyRadius2);
                    outerEdges[s_OutEdgeIndex][1] = MathF.Atan2(outerPoint.Y, outerPoint.X);
                    return new(new(ToVector3(outerPoint), ToVector3(rightCircumcenter.Value)), -1, s_OutEdgeIndex);
                }
                return new(null, -1, -1);
            }
            return new(null, -1, -1);
        }

        private static Vector2? GetCircumcenter(StarSystem starSystem, StarSystem left, StarSystem right)
        {
            if (left.Neighbors!.Contains(right))
            {
                return VoronoiGrapher.GetCircumcenter(starSystem.Position.Xz, left.Position.Xz, right.Position.Xz);
            }
            return null;
        }

        private static Vector3 ToVector3(Vector2 v)
        {
            return new(v.X, 0, v.Y);
        }
    }
}
