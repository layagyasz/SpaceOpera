using Cardamom.Graphics;
using Cardamom.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SpaceOpera.Core;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Common;
using SpaceOpera.View.Scenes.Highlights;

namespace SpaceOpera.View.StarSystemViews
{
    public class StarSystemViewFactory
    {
        private static readonly float s_BorderWidth = 0.01f;

        private static readonly float s_LocalOrbitScale = 0.25f;
        private static readonly float s_LocalOrbitY = -0.1f;

        private static readonly Color4 s_PinColor = new(0.7f, 0.7f, 0.7f, 1f);
        private static readonly float s_PinDashLength = 0.01f;
        private static readonly Interval s_PinYRange = new(-0.25f, -0.05f);
        private static readonly float s_PinScale = 0.001f;

        private static readonly float s_SolarOrbitY = -0.25f;

        public RenderShader BorderShader { get; }
        public RenderShader FillShader { get; }
        public RenderShader PinShader { get; }

        public StarSystemViewFactory(RenderShader borderShader, RenderShader fillShader, RenderShader pinShader)
        {
            BorderShader = borderShader;
            FillShader = fillShader;
            PinShader = pinShader;
        }

        public StarSubSystemRig Create(
            SolarOrbitRegion orbit, StarCalendar calendar, float orbitScale, float radius, float scale)
        {
            return new(orbit.LocalOrbit.StellarBody, calendar, Create(orbit, radius, scale), orbitScale);
        }

        public StarSubSystemView Create(SolarOrbitRegion orbit, float radius, float scale)
        {
            Vertex3[] pin = new Vertex3[2];
            pin[0] = new(scale * new Vector3(0, s_PinYRange.Minimum, 0), s_PinColor, new());
            pin[1] = new(scale * new Vector3(0, s_PinYRange.Maximum, 0), s_PinColor, new());
            var pinBuffer = new VertexBuffer<Vertex3>(PrimitiveType.Lines);
            pinBuffer.Buffer(pin, 0, 2);

            var bounds = new Dictionary<INavigable, SpaceSubRegionBounds>
            {
                { orbit, OrbitBounds.ComputeBounds(scale * s_SolarOrbitY, radius, scale) },
                { 
                    orbit.LocalOrbit,
                    OrbitBounds.ComputeBounds(scale * s_LocalOrbitY, s_LocalOrbitScale * radius, scale) 
                }
            };
            var highlight =
                new HighlightLayer<INavigable>(
                    bounds.Keys,
                    bounds,
                    scale * s_BorderWidth,
                    Matrix4.Identity,
                    BorderShader,
                    FillShader);
            return new(highlight, new(pinBuffer, PinShader, scale * s_PinScale, scale * s_PinDashLength));
        }
    }
}
