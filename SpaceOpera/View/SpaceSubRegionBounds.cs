using Cardamom.Mathematics.Geometry;
using OpenTK.Mathematics;

namespace SpaceOpera.View
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
        public Edge[] NeighborEdges;
        public Line3[] OuterEdges;

        private SpaceSubRegionBounds(Vector3 center, Edge[] neighborEdges, Line3[] outerEdges)
        {
            NeighborEdges = neighborEdges;
            OuterEdges = outerEdges;
        }

        public static SpaceSubRegionBounds FromEdges(
            Vector3 center, Edge[] neighborEdges, Line3[] outerEdges)
        {
            return new(center, neighborEdges, outerEdges);
        }
    }
}
