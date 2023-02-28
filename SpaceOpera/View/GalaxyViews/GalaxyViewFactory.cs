using Cardamom.Collections;
using Cardamom.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.StarViews;

namespace SpaceOpera.View.GalaxyViews
{
    public class GalaxyViewFactory
    {
        private static readonly Color4 s_PinColor = new(0.7f, 0.7f, 0.7f, 1f);
        private static readonly float s_PinDashLength = 128f;
        private static readonly float s_PinPadding = 128f;
        private static readonly float s_PinY = -700f;
        private static readonly float s_PinScale = 8f;

        private static readonly float s_StarScale = 4096f;

        private static readonly Color4 s_TransitColor = new(0.5f, 0.5f, 0.7f, 1f);
        private static readonly Vector3 s_TransitOffset = new(0, -512f, 0);
        private static readonly float s_TransitPadding = 0f;
        private static readonly float s_TransitScale = 96f;

        public StarViewFactory StarViewFactory { get; }
        public RenderShader TransitShader { get; }
        public RenderShader PinShader { get; }

        public GalaxyViewFactory(
            StarViewFactory starViewFactory,
            RenderShader transitShader,
            RenderShader pinShader)
        {
            StarViewFactory = starViewFactory;
            TransitShader = transitShader;
            PinShader = pinShader;
        }

        public GalaxyModel CreateModel(Galaxy galaxy, float scale)
        {
            ArrayList<Vertex3> transits = new(2 * galaxy.Systems.Count);
            foreach (var transit in galaxy.GetTransits())
            {
                Vector3 d = transit.Item2.Position - transit.Item1.Position;
                float l = d.Length;
                d.Normalize();
                var segment =
                    Utils.CreateSegment(
                        new(scale * (transit.Item1.Position + s_TransitPadding * d + s_TransitOffset), d),
                        scale * (l - 2 * s_TransitPadding),
                        Vector3.UnitY, 
                        scale * s_TransitScale,
                        true);
                Utils.AddVertices(transits, segment, s_TransitColor);
            }
            var transitBuffer = new VertexBuffer<Vertex3>(PrimitiveType.Triangles);
            transitBuffer.Buffer(transits.GetData(), 0, transits.Count);

            ArrayList<Vertex3> pins = new(2 * galaxy.Systems.Count);
            foreach (var system in galaxy.Systems)
            {
                pins.Add(new(scale * new Vector3(system.Position.X, s_PinY, system.Position.Z), s_PinColor, new()));
                pins.Add(
                    new(
                        scale * new Vector3(system.Position.X, system.Position.Y - s_PinPadding, system.Position.Z),
                        s_PinColor,
                        new()));
            }
            var pinBuffer = new VertexBuffer<Vertex3>(PrimitiveType.Lines);
            pinBuffer.Buffer(pins.GetData(), 0, pins.Count);

            return new GalaxyModel(
                StarViewFactory.CreateView(galaxy.Systems.Select(x => x.Star),
                    galaxy.Systems.Select(x => scale * x.Position),
                    s_StarScale * scale,
                    /* depthTest= */ false),
                new(transitBuffer, TransitShader),
                new(pinBuffer, PinShader, scale * s_PinScale, scale * s_PinDashLength));
        }
    }
}
