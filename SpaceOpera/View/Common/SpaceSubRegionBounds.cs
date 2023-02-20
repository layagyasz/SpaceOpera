using Cardamom.Mathematics.Geometry;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Common
{
    public class SpaceSubRegionBounds
    {
        public class Edge
        {
            public Segment3? Segment { get; }
            public int LeftOuterEdge { get; }
            public int RightOuterEdge { get; }

            public Edge(Segment3? segment, int leftOuterEdge, int rightOuterEdge)
            {
                Segment = segment;
                LeftOuterEdge = leftOuterEdge;
                RightOuterEdge = rightOuterEdge;
            }
        }

        public Vector3 Center { get; }
        public Vector3 Axis { get; }
        public SpaceSubRegionBounds[]? Neighbors { get; private set; }
        public Edge[] NeighborEdges;
        public Line3?[] OuterEdges;

        private SpaceSubRegionBounds(Vector3 center, Vector3 axis, Edge[] neighborEdges, Line3?[] outerEdges)
        {
            Center = center;
            Axis = axis;
            NeighborEdges = neighborEdges;
            OuterEdges = outerEdges;
        }

        public static SpaceSubRegionBounds FromEdges(
            Vector3 center, Vector3 axis, Edge[] neighborEdges, Line3[] outerEdges)
        {
            return new(center, axis, neighborEdges, outerEdges);
        }

        public static Dictionary<T, SpaceSubRegionBounds> CreateBounds<T>(
            IEnumerable<T> objects, Func<T, SpaceSubRegionBounds> boundsFn, Func<T, IEnumerable<T>> neighborsFn)
            where T : notnull
        {
            var dict = new Dictionary<T, SpaceSubRegionBounds>();
            foreach (var obj in objects)
            {
                dict.Add(obj, boundsFn(obj));
            }
            foreach (var entry in dict)
            {
                entry.Value.SetNeighbors(neighborsFn(entry.Key).Select(x => dict[x]));
            }
            return dict;
        }

        public void SetNeighbors(IEnumerable<SpaceSubRegionBounds> neighbors)
        {
            Neighbors = neighbors.ToArray();
        }
    }
}
