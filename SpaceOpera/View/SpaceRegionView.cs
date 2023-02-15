using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using static SpaceOpera.View.SpaceSubRegionBounds;

namespace SpaceOpera.View
{
    public class SpaceRegionView : GraphicsResource, IRenderable
    {
        private static readonly float s_Alpha = 0.25f;
        private static readonly float s_FillOffset = -0.0001f;

        private VertexBuffer<Vertex3>? _vertices;
        private readonly RenderShader _shader;

        public SpaceRegionView(VertexBuffer<Vertex3> vertices, RenderShader shader)
        {
            _vertices = vertices;
            _shader = shader;
        }

        public static SpaceRegionView Create(
            RenderShader shader,
            ISet<SpaceSubRegionBounds> subRegions,
            Color4 borderColor,
            Color4 color,
            float borderWidth,
            bool mergeSubRegions = true)
        {
            var vertices = new ArrayList<Vertex3>();
            TraceBounds(vertices, subRegions, borderColor, color, borderWidth, mergeSubRegions);
            var buffer = new VertexBuffer<Vertex3>(PrimitiveType.Triangles);
            buffer.Buffer(vertices.GetData(), 0, vertices.Count);
            return new SpaceRegionView(buffer, shader);
        }

        public static void TraceBounds(
            ArrayList<Vertex3> vertices,
            ISet<SpaceSubRegionBounds> subRegions,
            Color4 borderColor,
            Color4 color,
            float borderWidth,
            bool mergeSubRegions)
        {
            color.A *= s_Alpha;
            foreach (var bounds in subRegions)
            {
                for (int i = 0; i < bounds.NeighborEdges.Length; ++i)
                {
                    var edge = bounds.NeighborEdges[i];
                    if (edge.Segment != null)
                    {
                        if (color.A > 0)
                        {
                            var offset = s_FillOffset * bounds.Axis;
                            vertices.Add(new(bounds.Center + offset, color, new()));
                            vertices.Add(new(edge.Segment.Value.Left + offset, color, new()));
                            vertices.Add(new(edge.Segment.Value.Right + offset, color, new()));
                        }
                        if (!mergeSubRegions || !subRegions.Contains(bounds.Neighbors![i]))
                        {
                            int leftIndex =
                                GetNextValidEdge(bounds.NeighborEdges, i + bounds.Neighbors!.Length - 1, -1);
                            int rightIndex = GetNextValidEdge(bounds.NeighborEdges, i + 1, 1);

                            bool leftInner =
                                !mergeSubRegions
                                || !subRegions.Contains(bounds.Neighbors[leftIndex])
                                || edge.LeftOuterEdge > -1;
                            bool rightInner =
                                !mergeSubRegions
                                || !subRegions.Contains(bounds.Neighbors[rightIndex])
                                || edge.RightOuterEdge > -1;

                            AddEdge(
                                borderWidth,
                                bounds.NeighborEdges[i].Segment!.Value,
                                edge.LeftOuterEdge == -1 
                                    ? bounds.NeighborEdges[leftIndex].Segment!.Value 
                                    : bounds.OuterEdges[edge.LeftOuterEdge]!.GetSegment(
                                        bounds.OuterEdges[edge.LeftOuterEdge]!.Count - 2),
                                leftInner,
                                edge.RightOuterEdge == -1 
                                    ? bounds.NeighborEdges[rightIndex].Segment!.Value 
                                    : bounds.OuterEdges[edge.RightOuterEdge]!.GetSegment(0),
                                rightInner,
                                borderColor,
                                vertices);
                        }
                    }
                }
                for (int i = 0; i < bounds.OuterEdges.Length; ++i)
                {
                    if (bounds.OuterEdges[i] == null)
                    {
                        continue;
                    }

                    var leftIndex = GetOuterLeftEdge(bounds.NeighborEdges, i);
                    var rightIndex = GetOuterRightEdge(bounds.NeighborEdges, i);
                    var leftEdge = leftIndex > -1 ? bounds.NeighborEdges[leftIndex] : null;
                    var rightEdge = rightIndex > -1 ? bounds.NeighborEdges[rightIndex] : null;

                    for (int j = 0; j < bounds.OuterEdges[i]!.Count - 1; ++j)
                    {
                        var segment = bounds.OuterEdges[i]!.GetSegment(j);

                        if (color.A > 0)
                        {
                            var offset = s_FillOffset * bounds.Axis;
                            vertices.Add(new(bounds.Center + offset, color, new()));
                            vertices.Add(new(segment.Left + offset, color, new()));
                            vertices.Add(new(segment.Right + offset, color, new()));
                        }

                        bool leftInner =
                            !mergeSubRegions ||
                            leftEdge == null ||
                            (j != 0 && !subRegions.Contains(bounds.Neighbors![leftIndex]));
                        bool rightInner =
                            !mergeSubRegions ||
                            rightEdge == null || 
                                (j != bounds.OuterEdges[i]!.Count - 1) 
                                    && !subRegions.Contains(bounds.Neighbors![rightIndex]);

                        AddEdge(
                            borderWidth,
                            segment,
                            j == 0 ? leftEdge!.Segment!.Value : bounds.OuterEdges[i]!.GetSegment(j - 1),
                            leftInner,
                            j == bounds.OuterEdges[i]!.Count - 2 
                                ? rightEdge!.Segment!.Value : bounds.OuterEdges[i]!.GetSegment(j + 1),
                            rightInner,
                            borderColor,
                            vertices);
                    }
                }
            }
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 context) { }

        public void Draw(RenderTarget target, UiContext context)
        {
            target.Draw(_vertices!, 0, _vertices!.Length, new(BlendMode.Alpha, _shader));
        }

        public void Update(long delta) { }

        protected override void DisposeImpl()
        {
            _vertices!.Dispose();
            _vertices = null;
        }

        private static int GetOuterLeftEdge(Edge[] neighbors, int edge)
        {
            for (int i = 0; i < neighbors.Length; ++i)
            {
                if (neighbors[i].RightOuterEdge == edge)
                {
                    return i;
                }
            }
            return -1;
        }

        private static int GetOuterRightEdge(Edge[] neighbors, int edge)
        {
            for (int i = 0; i < neighbors.Length; ++i)
            {
                if (neighbors[i].LeftOuterEdge == edge)
                {
                    return i;
                }
            }
            return -1;
        }

        private static int GetNextValidEdge(SpaceSubRegionBounds.Edge[] edges, int startIndex, int increment)
        {
            int i = startIndex % edges.Length;
            while (true)
            {
                if (edges[i].Segment != null)
                {
                    return i;
                }
                i = (i + increment + edges.Length) % edges.Length;
            }
            throw new InvalidOperationException();
        }

        private static void AddEdge(
            float borderWidth,
            Segment3 edge,
            Segment3 left,
            bool leftInner,
            Segment3 right,
            bool rightInner,
            Color4 color,
            ArrayList<Vertex3> vertices)
        {
            var leftDir = left.Left - left.Right;
            leftDir.Normalize();
            if (leftInner)
            {
                leftDir = 0.5f * (leftDir + (edge.Right - edge.Left).Normalized());
            }
            var rightDir = right.Right - right.Left;
            rightDir.Normalize();
            if (rightInner)
            {
                rightDir = 0.5f * (rightDir + (edge.Left - edge.Right).Normalized());
            }
            var segment =
                Utils.CreateSegment(edge.Left, edge.Right, leftDir, rightDir, borderWidth, false);
            Utils.AddVertices(vertices, segment, color);
        }
    }
}
