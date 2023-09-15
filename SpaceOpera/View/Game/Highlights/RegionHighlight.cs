using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SpaceOpera.View.Game.Common;

namespace SpaceOpera.View.Game.Highlights
{
    public abstract class RegionHighlight : IHighlight
    {
        private static readonly float s_Alpha = 0.25f;

        private readonly record struct EdgeKey(SpaceSubRegionBounds Key, int Index, bool IsOuter);

        public abstract EventHandler<EventArgs>? Updated { get; set; }

        public abstract bool Merge { get; }
        public abstract float BorderWidth { get; }
        public abstract Color4 BorderColor { get; }
        public abstract Color4 Color { get; }
        public abstract bool Contains(object @object);
        public abstract void Unhook();

        public IRenderable CreateHighlight<TRange>(
            HighlightShaders shaders, 
            IEnumerable<BoundsAndRegionKey> range,
            float borderWidth)
        {
            var outlineVertices = new ArrayList<Vertex3>();
            var fillVertices = new ArrayList<Vertex3>();
            TraceBounds(
                outlineVertices,
                fillVertices,
                range.Where(x => Contains(x.RegionKey)).ToDictionary(x => x.Bounds, x => x.RegionKey),
                BorderColor, 
                Color,
                borderWidth * BorderWidth,
                Merge);
            var outlineBuffer = new VertexBuffer<Vertex3>(PrimitiveType.Triangles);
            outlineBuffer.Buffer(outlineVertices.GetData(), 0, outlineVertices.Count);
            var fillBuffer = new VertexBuffer<Vertex3>(PrimitiveType.Triangles);
            fillBuffer.Buffer(fillVertices.GetData(), 0, fillVertices.Count);
            return new SpaceRegionView(outlineBuffer, shaders.Outline, fillBuffer, shaders.Fill);
        }

        private static void TraceBounds(
            ArrayList<Vertex3> outline,
            ArrayList<Vertex3> fill,
            IDictionary<SpaceSubRegionBounds, object> subRegions,
            Color4 borderColor,
            Color4 color,
            float borderWidth,
            bool mergeSubRegions)
        {
            color.A *= s_Alpha;
            var visited = new HashSet<EdgeKey>();
            foreach (var kvp in subRegions)
            {
                var bounds = kvp.Key;
                for (int i = 0; i < bounds.NeighborEdges.Length; ++i)
                {
                    if (color.A > 0)
                    {
                        var edge = bounds.NeighborEdges[i];
                        if (edge.Segment != null)
                        {
                            fill.Add(new(bounds.Center, color, new()));
                            fill.Add(new(edge.Segment.Value.Left, color, new()));
                            fill.Add(new(edge.Segment.Value.Right, color, new()));
                        }
                    }

                    var key = new EdgeKey(bounds, i, false);
                    if (bounds.NeighborEdges[i].Segment != null
                        && DrawEdge(kvp.Value, bounds.Neighbors![i], subRegions, mergeSubRegions)
                        && !visited.Contains(key))
                    {
                        Utils.AddVertices(
                            outline,
                            borderColor,
                            TraceFrom(key, subRegions, mergeSubRegions, visited),
                            borderWidth,
                            /* center= */ false);
                    }
                }
                for (int i = 0; i < bounds.OuterEdges.Length; ++i)
                {
                    if (bounds.OuterEdges[i] == null)
                    {
                        continue;
                    }

                    bool loop = bounds.OuterEdges[i]!.IsLoop;
                    int count = bounds.OuterEdges[i]!.Count - (loop ? 0 : 1);
                    for (int j = 0; j < count; ++j)
                    {
                        var segment = bounds.OuterEdges[i]!.GetSegment(j);
                        if (color.A > 0)
                        {
                            fill.Add(new(bounds.Center, color, new()));
                            fill.Add(new(segment.Left, color, new()));
                            fill.Add(new(segment.Right, color, new()));
                        }
                    }

                    var key = new EdgeKey(bounds, i, true);
                    if (!visited.Contains(key))
                    {
                        Utils.AddVertices(
                            outline,
                            borderColor,
                            loop ? bounds.OuterEdges[i]! : TraceFrom(key, subRegions, mergeSubRegions, visited),
                            borderWidth,
                            /* center= */ false);
                    }
                }
            }
        }

        private static Line3 TraceFrom(
            EdgeKey start,
            IDictionary<SpaceSubRegionBounds, object> subRegions,
            bool mergeSubRegions,
            ISet<EdgeKey> visited)
        {
            var current = start;
            var builder = new Line3.Builder().IsLoop();
            do
            {
                visited.Add(current);
                if (current.IsOuter)
                {
                    foreach (var point in current.Key.OuterEdges[current.Index]!.Skip(1))
                    {
                        builder.AddPoint(point, current.Key.Axis);
                    }
                }
                else
                {
                    var edge = current.Key.NeighborEdges[current.Index];
                    if (edge.Segment != null)
                    {
                        builder.AddPoint(edge.Segment.Value.Right, edge.Normal!.Value.Right);
                    }
                }
                current = Step(current, subRegions, mergeSubRegions);
            }
            while (!start.Equals(current));
            return builder.Build();
        }

        private static EdgeKey Step(
            EdgeKey current,
            IDictionary<SpaceSubRegionBounds, object> subRegions,
            bool mergeSubRegions)
        {
            var regionKey = subRegions[current.Key];
            int nextIndex;
            if (current.IsOuter)
            {
                nextIndex = Array.FindIndex(current.Key.NeighborEdges, x => x.LeftOuterEdge == current.Index);
                if (DrawEdge(regionKey, current.Key.Neighbors![nextIndex], subRegions, mergeSubRegions))
                {
                    return new(current.Key, nextIndex, /* isOuter= */ false);
                }
                return new(current.Key.Neighbors![nextIndex], current.Index, /* isOuter= */ true);
            }
            var edge = current.Key.NeighborEdges[current.Index];
            if (edge.RightOuterEdge >= 0)
            {
                return new(current.Key, edge.RightOuterEdge, /* isOuter= */ true);
            }
            nextIndex = StepIndex(current.Index, current.Key.NeighborEdges.Length);
            if (DrawEdge(regionKey, current.Key.Neighbors![nextIndex], subRegions, mergeSubRegions))
            {
                return new(current.Key, nextIndex, /* isOuter= */ false);
            }
            var neighbor = current.Key.Neighbors[nextIndex];
            return new(
                neighbor,
                StepIndex(Array.IndexOf(neighbor.Neighbors!, current.Key), neighbor.Neighbors!.Length),
                /* isOuter= */ false);
        }

        private static int StepIndex(int index, int mod)
        {
            return (index + 1) % mod;
        }

        private static bool DrawEdge(
            object regionKey,
            SpaceSubRegionBounds neighbor,
            IDictionary<SpaceSubRegionBounds, object> subRegions,
            bool mergeSubRegions)
        {
            var containsNeighbor = subRegions.TryGetValue(neighbor, out var r);
            return !containsNeighbor || !(mergeSubRegions || regionKey == r);
        }
    }
}
