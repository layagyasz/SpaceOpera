﻿using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Mathematics;
using Cardamom.Mathematics.Geometry;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SpaceOpera.Controller;
using SpaceOpera.Controller.Game.Scenes;
using SpaceOpera.Core;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Game.Common;
using SpaceOpera.View.Game.FormationViews;
using SpaceOpera.View.Game.Highlights;
using SpaceOpera.View.Game.StarViews;
using SpaceOpera.View.Game.StellarBodyViews;

namespace SpaceOpera.View.Game.StarSystemViews
{
    public class StarSystemViewFactory
    {
        private static readonly float s_BorderWidth = 0.01f;
        private static readonly float s_GuidelineScale = 0.0048f;
        private static readonly Color4 s_GuidelineGoldilocksColor = new(0.5f, 0.7f, 0.5f, 1f);
        private static readonly Color4 s_GuidelineTransitColor = new(0.7f, 0.5f, 0.7f, 1f);
        private static readonly Color4 s_GuidelineViableColor = new(0.7f, 0.5f, 0.5f, 1f);
        private static readonly float s_GuidelineResolution = 0.02f * MathHelper.Pi;
        private static readonly float s_LocalOrbitScale = 0.5f;
        private static readonly Color4 s_OrbitColor = new(0.5f, 0.5f, 0.7f, 1f);
        private static readonly Color4 s_PinColor = new(0.7f, 0.7f, 0.7f, 1f);
        private static readonly float s_PinDashLength = 0.01f;
        private static readonly Interval s_PinYRange = new(-0.25f, 0f);
        private static readonly float s_PinScale = 0.0004f;
        private static readonly float s_StarScale = 2f;
        private static readonly float s_StellarBodyScale = 0.01f;


        private static readonly Interval s_RadiusRange = new(0.1f, float.PositiveInfinity);

        private static readonly float s_LocalOrbitY = -0.1f;
        private static readonly float s_SolarOrbitY = -0.25f;

        public StarViewFactory StarViewFactory { get; }
        public StellarBodyViewFactory StellarBodyViewFactory { get; }
        public FormationLayerFactory FormationLayerFactory { get; }
        public RenderShader GuidelineShader { get; }
        public RenderShader PinShader { get; }
        public HighlightShaders HighlightShaders { get; }

        public StarSystemViewFactory(
            StarViewFactory starViewFactory,
            StellarBodyViewFactory stellarBodyViewFactory,
            FormationLayerFactory formationLayerFactory,
            RenderShader guidelineShader,
            RenderShader pinShader,
            HighlightShaders highlightShaders)
        {
            StarViewFactory = starViewFactory;
            StellarBodyViewFactory = stellarBodyViewFactory;
            FormationLayerFactory = formationLayerFactory;
            GuidelineShader = guidelineShader;
            PinShader = pinShader;
            HighlightShaders = highlightShaders;
        }

        public StarSystemModel Create(StarSystem starSystem, float scale)
        {
            var starBuffer =
                StarViewFactory.CreateView(
                    Enumerable.Repeat(starSystem.Star, 1),
                    Enumerable.Repeat(new Vector3(), 1),
                    scale * s_StarScale,
                    /* depthTest= */ true);
            var guidelines = new ArrayList<Vertex3>();
            AddGuideline(guidelines, starSystem.ViableRange.Maximum, s_GuidelineViableColor, scale);
            AddGuideline(guidelines, starSystem.ViableRange.Minimum, s_GuidelineViableColor, scale);
            AddGuideline(guidelines, starSystem.GoldilocksRange.Minimum, s_GuidelineGoldilocksColor, scale);
            AddGuideline(guidelines, starSystem.GoldilocksRange.Maximum, s_GuidelineGoldilocksColor, scale);
            AddGuideline(guidelines, starSystem.TransitLimit, s_GuidelineTransitColor, scale);
            for (int i = 0; i < starSystem.Orbiters.Count; ++i)
            {
                AddGuideline(
                    guidelines,
                    x => scale * MathF.Log((float)starSystem.Orbiters[i].Orbit.GetDistance(x) + 1),
                    s_OrbitColor,
                    scale);
            }
            var guidelineBuffer = new VertexBuffer<Vertex3>(PrimitiveType.Triangles);
            guidelineBuffer.Buffer(guidelines.GetData(), 0, guidelines.Count);
            return new(starBuffer, new(guidelineBuffer, GuidelineShader));
        }

        public StarSubSystemRig Create(SolarOrbitRegion orbit, StarCalendar calendar, float radius, float scale)
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
            var stellarBody =
                StellarBodyViewFactory.Create(
                    orbit.LocalOrbit.StellarBody, s_StellarBodyScale * scale, false).GetNow();
            var interactors = new SubRegionInteractor[]
            {
                new(
                    new SubRegionController(orbit),
                    new Disk(scale * new Vector3(0, s_SolarOrbitY, 0), Vector3.UnitY, scale * radius)),
                new(
                    new SubRegionController(orbit.LocalOrbit),
                    new Disk(
                        scale * new Vector3(0, s_LocalOrbitY, 0), Vector3.UnitY, scale * s_LocalOrbitScale * radius)),
                new(new SubRegionController(orbit.LocalOrbit.StellarBody), new Sphere(new(), stellarBody.Radius))
            };

            var highlight =
                HighlightLayer<SolarOrbitRegion, INavigable>.Create(
                    orbit,
                    bounds.Keys,
                    Identity,
                    bounds,
                    scale * s_BorderWidth,
                    Matrix4.Identity,
                    HighlightShaders);

            var formationLayer = FormationLayerFactory.CreateForSubSystem(orbit);
            return new(
                new RigController(interactors.Select(x => x.Controller).Cast<IActionController>().ToArray()),
                orbit.LocalOrbit.StellarBody,
                calendar,
                new(
                    stellarBody,
                    highlight,
                    formationLayer,
                    new(pinBuffer, PinShader, scale * s_PinScale, scale * s_PinDashLength)),
                interactors,
                scale);
        }

        private static void AddGuideline(ArrayList<Vertex3> vertices, float radius, Color4 color, float scale)
        {
            if (!float.IsNaN(radius))
            {
                var r = scale * MathF.Log(radius + 1);
                AddGuideline(vertices, x => r, color, scale);
            }
        }

        private static void AddGuideline(
            ArrayList<Vertex3> vertices, Func<float, float> radiusFn, Color4 color, float scale)
        {
            Utils.AddVertices(
                vertices,
                color,
                new Line3(
                    Shape.GetCirclePoints(radiusFn, s_GuidelineResolution)
                        .Select(x => new Vector3(x.X, 0, x.Y)).ToArray(),
                    Vector3.UnitY,
                    true),
                scale * s_GuidelineScale,
                true);
        }

        private static IEnumerable<T> Identity<T>(T @object)
        {
            yield return @object;
        }
    }
}
