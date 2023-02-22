using Cardamom.Graphics;
using Cardamom.Mathematics;
using Cardamom.Mathematics.Geometry;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SpaceOpera.Controller.Scenes;
using SpaceOpera.Core;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Common;
using SpaceOpera.View.Common.Highlights;

namespace SpaceOpera.View.StarSystemViews
{
    public class StarSystemViewFactory
    {
        private static readonly float s_BorderWidth = 0.01f;

        private static readonly float s_LocalOrbitScale = 0.5f;
        private static readonly float s_LocalOrbitY = -0.1f;

        private static readonly Color4 s_PinColor = new(0.7f, 0.7f, 0.7f, 1f);
        private static readonly float s_PinDashLength = 0.01f;
        private static readonly Interval s_PinYRange = new(-0.25f, -0.05f);
        private static readonly float s_PinScale = 0.0004f;

        private static readonly Interval s_RadiusRange = new(0.1f, float.PositiveInfinity);

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
            SolarOrbitRegion orbit, StarCalendar calendar, float radius, float scale)
        {
            Vertex3[] pin = new Vertex3[2];
            pin[0] = new(scale * new Vector3(0, s_PinYRange.Minimum, 0), s_PinColor, new());
            pin[1] = new(scale * new Vector3(0, s_PinYRange.Maximum, 0), s_PinColor, new());
            var pinBuffer = new VertexBuffer<Vertex3>(PrimitiveType.Lines);
            pinBuffer.Buffer(pin, 0, 2);

            radius = s_RadiusRange.Clamp(radius);
            var bounds = new Dictionary<INavigable, SpaceSubRegionBounds>
            {
                { orbit, StarSystemSubRegionBounds.ComputeBounds(new(0, s_SolarOrbitY, 0), radius, scale) },
                {
                    orbit.LocalOrbit,
                    StarSystemSubRegionBounds.ComputeBounds(
                        new(0, s_LocalOrbitY, 0), s_LocalOrbitScale * radius, scale)
                }
            };
            var interactors = new SubRegionInteractor[]
            {
                new(
                    new SubRegionController(orbit),
                    new Disk(scale * new Vector3(0, s_SolarOrbitY, 0), Vector3.UnitY, scale * radius)),
                new(
                    new SubRegionController(orbit.LocalOrbit),
                    new Disk(
                        scale * new Vector3(0, s_LocalOrbitY, 0), Vector3.UnitY, scale * s_LocalOrbitScale * radius))
            };
            var highlight =
                new HighlightLayer<INavigable>(
                    bounds.Keys,
                    bounds,
                    scale * s_BorderWidth,
                    Matrix4.Identity,
                    BorderShader,
                    FillShader);
            return new(
                new RigController(interactors.Select(x => x.Controller).Cast<ISceneController>().ToArray()),
                orbit.LocalOrbit.StellarBody,
                calendar, 
                new(highlight, new(pinBuffer, PinShader, scale * s_PinScale, scale * s_PinDashLength)),
                interactors,
                scale);
        }
    }
}
