using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SpaceOpera.Core;
using SpaceOpera.Core.Military.Fronts;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Game.Common;

namespace SpaceOpera.View.Game.Highlights
{
    public class FrontHighlight : IHighlight
    {
        private readonly static float s_BorderWidth = 16f;
        private readonly static Color4 s_DefaultColor = new(0.5f, 0.5f, 0.5f, 0.5f);
        private readonly static Color4 s_EnemyColor = new(1f, 0f, 0f, 0.5f);

        public EventHandler<EventArgs>? Updated { get; set; }

        public Faction Faction { get; }
        public FrontManager FrontManager { get; }
        public DiplomaticRelationGraph DiplomaticRelationGraph { get; }

        public FrontHighlight(
            Faction faction, FrontManager frontManager, DiplomaticRelationGraph diplomaticRelationGraph)
        {
            Faction = faction;
            FrontManager = frontManager;
            DiplomaticRelationGraph = diplomaticRelationGraph;
        }

        public static ICompositeHighlight Create(Faction faction, World world)
        {
            return SimpleHighlight.Wrap(new FrontHighlight(faction, world.FrontManager, world.DiplomaticRelations));
        }

        public IRenderable CreateHighlight<TDomain, TRange>(
            HighlightShaders shaders,
            TDomain domain,
            IDictionary<TRange, BoundsAndRegionKey> range,
            float borderWidth)
            where TDomain : notnull 
            where TRange : notnull
        {
            var outlineVertices = new ArrayList<Vertex3>();

            if (domain is StellarBody stellarBody)
            {
                borderWidth *= s_BorderWidth;
                foreach (var front in
                    FrontManager.Get(stellarBody).GetFronts().Where(x => x.Faction == Faction))
                {
                    Color4 color =
                        front.Opponent != null 
                        && front.Opponent != front.Faction 
                        && DiplomaticRelationGraph.CanAttack(front.Faction, front.Opponent)
                            ? s_EnemyColor
                            : s_DefaultColor;
                    Utils.AddVertices(
                        outlineVertices,
                        color,
                        Trace(front, range),
                        borderWidth,
                        /* center= */ false);
                }
            }

            var outlineBuffer = new VertexBuffer<Vertex3>(PrimitiveType.Triangles);
            outlineBuffer.Buffer(outlineVertices.GetData(), 0, outlineVertices.Count);
            var fillBuffer = new VertexBuffer<Vertex3>(PrimitiveType.Triangles);
            fillBuffer.Buffer(Array.Empty<Vertex3>(), 0, 0);
            return new SpaceRegionView(outlineBuffer, shaders.Outline, fillBuffer, shaders.Fill);
        }

        public void Hook(object domain)
        {
            if (domain is StellarBody body)
            {
                FrontManager.Get(body).Changed += HandleUpdate;
            }
        }

        public void Unhook(object domain)
        {
            if (domain is StellarBody body)
            {
                FrontManager.Get(body).Changed -= HandleUpdate;
            }
        }

        private void HandleUpdate(object? sender, EventArgs e)
        {
            Updated?.Invoke(this, e);
        }

        private static Line3 Trace<TRange>(Front front, IDictionary<TRange, BoundsAndRegionKey> range)
        {
            Line3.Builder builder = new();
            int i = 0;
            foreach (var edge in front.Edges)
            {
                var e = range[(TRange)(object)edge.Region].Bounds.NeighborEdges[edge.Index];
                if (i == 0)
                {
                    builder.AddPoint(e.Segment!.Value.Left, e.Normal!.Value.Left);
                }
                builder.AddPoint(e.Segment!.Value.Right, e.Normal!.Value.Right);
                ++i;
            }
            return builder.Build();
        }
    }
}
