using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using static SpaceOpera.View.Common.SpaceSubRegionBounds;

namespace SpaceOpera.View.Common
{
    public class SpaceRegionView : GraphicsResource, IRenderable
    {
        private static readonly float s_Alpha = 0.25f;

        private VertexBuffer<Vertex3>? _outline;
        private readonly RenderShader _outlineShader;
        private VertexBuffer<Vertex3>? _fill;
        private readonly RenderShader _fillShader;

        public SpaceRegionView(
            VertexBuffer<Vertex3> outline,
            RenderShader outlineShader,
            VertexBuffer<Vertex3> fill,
            RenderShader fillShader)
        {
            _outline = outline;
            _outlineShader = outlineShader;
            _fill = fill;
            _fillShader = fillShader;
        }

        public static SpaceRegionView Create(
            RenderShader outlineShader,
            RenderShader fillShader,
            ISet<SpaceSubRegionBounds> subRegions,
            Color4 borderColor,
            Color4 color,
            float borderWidth,
            bool mergeSubRegions)
        {
            var outlineVertices = new ArrayList<Vertex3>();
            var fillVertices = new ArrayList<Vertex3>();
            TraceBounds(outlineVertices, fillVertices, subRegions, borderColor, color, borderWidth, mergeSubRegions);
            var outlineBuffer = new VertexBuffer<Vertex3>(PrimitiveType.Triangles);
            outlineBuffer.Buffer(outlineVertices.GetData(), 0, outlineVertices.Count);
            var fillBuffer = new VertexBuffer<Vertex3>(PrimitiveType.Triangles);
            fillBuffer.Buffer(fillVertices.GetData(), 0, fillVertices.Count);
            return new SpaceRegionView(outlineBuffer, outlineShader, fillBuffer, fillShader);
        }

        public static void TraceBounds(
            ArrayList<Vertex3> outline,
            ArrayList<Vertex3> fill,
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
                            fill.Add(new(bounds.Center, color, new()));
                            fill.Add(new(edge.Segment.Value.Left, color, new()));
                            fill.Add(new(edge.Segment.Value.Right, color, new()));
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
                                outline);
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

                    int count = bounds.OuterEdges[i]!.Count - (bounds.OuterEdges[i]!.IsLoop ? 0 : 1);
                    for (int j = 0; j < count; ++j)
                    {
                        var segment = bounds.OuterEdges[i]!.GetSegment(j);

                        if (color.A > 0)
                        {
                            fill.Add(new(bounds.Center, color, new()));
                            fill.Add(new(segment.Left, color, new()));
                            fill.Add(new(segment.Right, color, new()));
                        }

                        bool leftInner =
                            !mergeSubRegions ||
                            leftEdge == null ||
                            j != 0 && !subRegions.Contains(bounds.Neighbors![leftIndex]);
                        bool rightInner =
                            !mergeSubRegions ||
                            rightEdge == null ||
                                j != bounds.OuterEdges[i]!.Count - 1
                                    && !subRegions.Contains(bounds.Neighbors![rightIndex]);
                        var leftSegment = 
                            leftEdge?.Segment ?? bounds.OuterEdges[i]!.GetSegment(bounds.OuterEdges[i]!.Count - 2);
                        var rightSegment = rightEdge?.Segment ?? bounds.OuterEdges[i]!.GetSegment(0);

                        AddEdge(
                            borderWidth,
                            segment,
                            j == 0 ? leftSegment : bounds.OuterEdges[i]!.GetSegment(j - 1),
                            leftInner,
                            j == count - 1 ? rightSegment : bounds.OuterEdges[i]!.GetSegment(j + 1),
                            rightInner,
                            borderColor,
                            outline);
                    }
                }
            }
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 context) { }

        public void Draw(RenderTarget target, UiContext context)
        {
            target.Draw(_fill!, 0, _fill!.Length, new(BlendMode.Alpha, _fillShader) { EnableDepthMask = false });
            target.Draw(
                _outline!, 0, _outline!.Length, new(BlendMode.Alpha, _outlineShader) { EnableDepthMask = false });
        }

        public void Update(long delta) { }

        protected override void DisposeImpl()
        {
            _outline!.Dispose();
            _outline = null;
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

        private static int GetNextValidEdge(Edge[] edges, int startIndex, int increment)
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
