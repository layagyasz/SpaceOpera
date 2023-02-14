using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Scenes.Highlights
{
    public class HighlightBuffer : GraphicsResource, IRenderable
    {
        private static readonly float s_Alpha = 0.25f;

        private VertexBuffer<Vertex3>? _vertices;
        private readonly RenderShader _shader;

        private HighlightBuffer(VertexBuffer<Vertex3> vertices, RenderShader shader)
        {
            _vertices = vertices;
            _shader = shader;
        }

        public static HighlightBuffer Create(
            RenderShader shader,
            ISet<SpaceSubRegionBounds> subRegions,
            Color4 borderColor,
            Color4 color,
            float borderWidth)
        {
            var bounds = TraceBounds(subRegions, borderColor, color, borderWidth);
            var buffer = new VertexBuffer<Vertex3>(PrimitiveType.Triangles);
            buffer.Buffer(bounds.GetData(), 0, bounds.Count);
            return new HighlightBuffer(buffer, shader);
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

        private static ArrayList<Vertex3> TraceBounds(
            ISet<SpaceSubRegionBounds> subRegions, Color4 borderColor, Color4 color, float borderWidth)
        {
            var vertices = new ArrayList<Vertex3>();
            color.A *= s_Alpha;
            foreach (var bounds in subRegions)
            {
                for (int i = 0; i < bounds.NeighborEdges.Length; ++i)
                {
                    var edge = bounds.NeighborEdges[i];
                    if (edge.Segment != null)
                    {
                        vertices.Add(new(bounds.Center, color, new()));
                        vertices.Add(new(edge.Segment.Value.Left, color, new()));
                        vertices.Add(new(edge.Segment.Value.Right, color, new()));
                        if (!subRegions.Contains(bounds.Neighbors![i]))
                        {
                            int leftIndex =
                                GetNextValidEdge(bounds.NeighborEdges, i + bounds.Neighbors.Length - 1, -1);
                            int rightIndex = GetNextValidEdge(bounds.NeighborEdges, i + 1, 1);

                            bool leftInner =
                                !subRegions.Contains(bounds.Neighbors[leftIndex])
                                || edge.LeftOuterEdge > -1;
                            bool rightInner =
                                !subRegions.Contains(bounds.Neighbors[rightIndex])
                                || edge.RightOuterEdge > -1;

                            AddEdge(
                                borderWidth,
                                bounds.NeighborEdges[i].Segment!.Value,
                                bounds.NeighborEdges[leftIndex].Segment!.Value,
                                leftInner,
                                bounds.NeighborEdges[rightIndex].Segment!.Value,
                                rightInner,
                                bounds.Center,
                                borderColor,
                                vertices);
                        }
                    }
                }
                for (int i = 0; i < bounds.OuterEdges.Length; ++i)
                {
                    var leftIndex = GetOuterLeftEdge(bounds.NeighborEdges, i);
                    var rightIndex = GetOuterRightEdge(bounds.NeighborEdges, i);
                    var leftEdge = leftIndex > -1 ? bounds.NeighborEdges[leftIndex] : null;
                    var rightEdge = rightIndex > -1 ? bounds.NeighborEdges[rightIndex] : null;

                    for (int j = 0; j < bounds.OuterEdges[i].Count; ++j)
                    {
                        var segment = bounds.OuterEdges[i].GetSegment(j);

                        vertices.Add(new(bounds.Center, color, new()));
                        vertices.Add(new(segment.Left, color, new()));
                        vertices.Add(new(segment.Right, color, new()));

                        bool leftInner =
                            leftEdge == null || (j != 0 && !subRegions.Contains(bounds.Neighbors![leftIndex]));
                        bool rightInner =
                            rightEdge == null
                            || (j != bounds.OuterEdges[i].Count - 1 
                                && !subRegions.Contains(bounds.Neighbors![rightIndex]));

                        AddEdge(
                            borderWidth,
                            segment,
                            leftEdge?.Segment,
                            leftInner,
                            rightEdge?.Segment,
                            rightInner,
                            bounds.Center,
                            borderColor,
                            vertices);
                    }
                }
            }
            return vertices;
        }

        private static int GetOuterLeftEdge(SpaceSubRegionBounds.Edge[] neighbors, int edge)
        {
            for (int i=0; i<neighbors.Length; ++i)
            {
                if (neighbors[i].RightOuterEdge == edge)
                {
                    return i;
                }
            }
            return -1;
        }

        private static int GetOuterRightEdge(SpaceSubRegionBounds.Edge[] neighbors, int edge)
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
            Segment3? left,
            bool leftInner,
            Segment3? right,
            bool rightInner,
            Vector3 center,
            Color4 color,
            ArrayList<Vertex3> vertices)
        {
            var leftAnchor = leftInner ? center : left!.Value.Right!;
            var rightAnchor = rightInner ? center : right!.Value.Left;

            var hinge = edge.Left + borderWidth * (leftAnchor - edge.Left).Normalized();

            vertices.Add(new(edge.Right, color, new()));
            vertices.Add(new(edge.Left, color, new()));
            vertices.Add(new(hinge, color, new()));
            vertices.Add(new(edge.Right, color, new()));
            vertices.Add(new(edge.Right + borderWidth * (rightAnchor - edge.Right).Normalized(), color, new()));
            vertices.Add(new(hinge, color, new()));
        }
    }
}
