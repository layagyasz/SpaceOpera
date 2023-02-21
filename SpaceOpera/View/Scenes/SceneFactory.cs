﻿using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Mathematics;
using Cardamom.Mathematics.Coordinates.Projections;
using Cardamom.Mathematics.Coordinates;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Spectra;
using SpaceOpera.View.GalaxyViews;
using SpaceOpera.View.StarViews;
using SpaceOpera.View.StellarBodyViews;
using OpenTK.Graphics.OpenGL4;
using Cardamom.ImageProcessing.Pipelines.Nodes;
using Cardamom.ImageProcessing.Pipelines;
using Cardamom.Utils.Suppliers;
using Cardamom.ImageProcessing;
using SpaceOpera.Controller.Scenes;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Scenes.Highlights;
using SpaceOpera.View.Common;
using SpaceOpera.View.StarSystemViews;
using SpaceOpera.Core;
using Cardamom.Collections;

namespace SpaceOpera.View.Scenes
{
    public class SceneFactory
    {
        private static readonly float s_GalaxyScale = 0.0001f;
        private static readonly Vector3 s_GalaxyFloor = new(0f, -700f, 0f);
        private static readonly float s_GalaxyRegionBorderWidth = 128f;

        private static readonly float s_LightPower = 1f;
        private static readonly Interval s_LuminanceRange = new(0.1f, float.MaxValue);

        private static readonly float s_SkyboxRadius = 1100;
        private static readonly int s_SkyboxPrecision = 64;
        private static readonly int s_SkyboxResolution = 2048;

        private static readonly float s_StarSystemGuidelineScale = 0.0048f;
        private static readonly Color4 s_StarSystemGuidelineColor = new(0.7f, 0.5f, 0.5f, 1f);
        private static readonly float s_StarSystemGuidelineResolution = 0.02f * MathHelper.Pi;
        private static readonly Color4 s_StarSystemOrbitColor = new(0.5f, 0.5f, 0.7f, 1f);
        private static readonly float s_StarSystemScale = 2f;
        private static readonly float s_StarSystemSceneStarScale = 0.25f;
        

        private static readonly float s_StellarBodySceneStarScale = 1024;
        private static readonly Vector3 s_StellarBodySceneStarPosition = new(0, 0, -1000);

        private static Skybox? _skyBox;

        public GalaxyViewFactory GalaxyViewFactory { get; }
        public StellarBodyViewFactory StellarBodyViewFactory { get; }
        public StarSystemViewFactory StarSystemViewFactory { get; }
        public StarViewFactory StarViewFactory { get; }
        public SpectrumSensitivity HumanEyeSensitivity { get; }
        public RenderShader SkyboxShader { get; }
        public RenderShader BorderShader { get; }
        public RenderShader FillShader { get; }
        public RenderShader GuidelineShader { get; }

        public SceneFactory(
            GalaxyViewFactory galaxyViewFactory,
            StellarBodyViewFactory stellarBodyViewFactory,
            StarSystemViewFactory starSystemViewFactory,
            StarViewFactory starViewFactory,
            SpectrumSensitivity humanEyeSensitivity,
            RenderShader skyboxShader, 
            RenderShader borderShader, 
            RenderShader fillShader,
            RenderShader guidelineShader)
        {
            GalaxyViewFactory = galaxyViewFactory;
            StellarBodyViewFactory = stellarBodyViewFactory;
            StarSystemViewFactory = starSystemViewFactory;
            StarViewFactory = starViewFactory;
            HumanEyeSensitivity = humanEyeSensitivity;
            SkyboxShader = skyboxShader;
            BorderShader = borderShader;
            FillShader = fillShader;
            GuidelineShader = guidelineShader;
        }

        public IGameScene Create(Galaxy galaxy)
        {
            float r = s_GalaxyScale * galaxy.Radius;
            var model = GalaxyViewFactory.CreateModel(galaxy, s_GalaxyScale);
            var galaxyController = GalaxyModelController.Create(galaxy, s_GalaxyScale);
            var interactiveModel = 
                new InteractiveModel(
                    model, 
                    new Disk(s_GalaxyScale * s_GalaxyFloor, Vector3.UnitY, r), 
                    galaxyController);
            var camera = new SubjectiveCamera3d(s_SkyboxRadius + 10);
            camera.OnCameraChange += (s, e) => model.Dirty();
            camera.SetDistance(0.05f);
            camera.SetPitch(-0.125f * MathHelper.Pi);
            camera.SetYaw(MathHelper.PiOver2);

            var bounds = SpaceSubRegionBounds.CreateBounds(
                galaxy.Systems,
                x => StarSystemBounds.ComputeBounds(x, galaxy.Radius, s_GalaxyScale), x => x.Neighbors!);
            var highlight =
                new HighlightLayer<StarSystem>(
                    galaxy.Systems,
                    bounds,
                    s_GalaxyScale * s_GalaxyRegionBorderWidth,
                    Matrix4.CreateTranslation(s_GalaxyScale * s_GalaxyFloor),
                    BorderShader, 
                    FillShader);

            var controller =
                new SceneController(
                    new GalaxyCameraController(camera)
                    {
                        Radius = r,
                        KeySensitivity = 0.0004f * r,
                        MouseWheelSensitivity = 0.02f * r,
                        PitchRange = new(-MathHelper.PiOver2 + 0.01f, -0.125f * MathHelper.Pi),
                        DistanceRange = new(0.05f, r)
                    },
                    galaxyController);
            _skyBox ??= CreateSkybox();
            return new GalaxyScene(controller, camera, interactiveModel, highlight, _skyBox);
        }

        public IGameScene Create(StarSystem starSystem, StarCalendar calendar)
        {
            var starBuffer = 
                StarViewFactory.CreateView(
                    Enumerable.Repeat(starSystem.Star, 1),
                    Enumerable.Repeat(new Vector3(), 1),
                    s_StarSystemSceneStarScale);

            var camera = new SubjectiveCamera3d(s_SkyboxRadius + 10);
            camera.OnCameraChange += (s, e) => starBuffer.Dirty();
            camera.SetDistance(0.05f);
            camera.SetPitch(-0.125f * MathHelper.Pi);
            camera.SetYaw(MathHelper.PiOver2);

            var distances = new float[starSystem.Orbiters.Count + 2];
            distances[0] = s_StarSystemScale * MathF.Log(starSystem.InnerBoundary + 1);
            distances[^2] = s_StarSystemScale * MathF.Log(starSystem.OuterBoundary + 1);
            distances[^1] = s_StarSystemScale * MathF.Log(starSystem.TransitLimit + 1);
            for (int i=0; i<starSystem.Orbiters.Count - 1; ++i)
            {
                distances[i + 1] =
                    s_StarSystemScale * 0.5f * MathF.Log((starSystem.Orbiters[i].Orbit.GetAverageDistance() + 1)
                    * (starSystem.Orbiters[i + 1].Orbit.GetAverageDistance() + 1));
            }
            var rigs = new StarSubSystemRig[starSystem.Orbiters.Count];
            var guidelines = new ArrayList<Vertex3>();
            AddGuideline(guidelines, distances[0], s_StarSystemGuidelineColor);
            AddGuideline(guidelines, distances[^1], s_StarSystemGuidelineColor);
            AddGuideline(guidelines, distances[^2], s_StarSystemGuidelineColor);
            for (int i=0; i<starSystem.Orbiters.Count; ++i)
            {
                var d = s_StarSystemScale * MathF.Log(starSystem.Orbiters[i].Orbit.GetAverageDistance() + 1);
                rigs[i] = 
                    StarSystemViewFactory.Create(
                        starSystem.OrbitalRegions[i],
                        calendar,
                        Math.Min(distances[i + 1] - d, d - distances[i]),
                        s_StarSystemScale);
                Utils.AddVertices(
                    guidelines,
                    s_StarSystemOrbitColor,
                    new Line3(
                        Shape.GetCirclePoints(
                            x => s_StarSystemScale * MathF.Log(starSystem.Orbiters[i].GetSolarOrbitDistance(x) + 1),
                            s_StarSystemGuidelineResolution)
                            .Select(x => new Vector3(x.X, 0, x.Y)).ToArray(),
                        true),
                    Vector3.UnitY, 
                    s_StarSystemScale * s_StarSystemGuidelineScale, 
                    true);
            }

            var r = s_StarSystemScale * MathF.Log(starSystem.TransitLimit + 1);
            var controller =
                new SceneController(
                    new GalaxyCameraController(camera)
                    {
                        Radius = r,
                        KeySensitivity = 0.0004f * r,
                        MouseWheelSensitivity = 0.02f * r,
                        PitchRange = new(-MathHelper.PiOver2 + 0.01f, -0.125f * MathHelper.Pi),
                        DistanceRange = new(0.05f, r)
                    },
                    new StarSystemController());
            _skyBox ??= CreateSkybox();
            var guidelineBuffer = new VertexBuffer<Vertex3>(PrimitiveType.Triangles);
            guidelineBuffer.Buffer(guidelines.GetData(), 0, guidelines.Count);
            return new StarSystemScene(
                controller, camera, starBuffer, rigs, new(guidelineBuffer, GuidelineShader), _skyBox);
        }

        private static void AddGuideline(ArrayList<Vertex3> vertices, float radius, Color4 color)
        {
            Utils.AddVertices(
                vertices,
                color,
                new Line3(
                    Shape.GetCirclePoints(x => radius, s_StarSystemGuidelineResolution)
                        .Select(x => new Vector3(x.X, 0, x.Y)).ToArray(),
                    true),
                Vector3.UnitY,
                s_StarSystemScale * s_StarSystemGuidelineScale,
                true);
        }

        public IGameScene Create(StellarBody stellarBody)
        {
            var model = StellarBodyViewFactory.CreateModel(stellarBody);
            var camera = new SubjectiveCamera3d(s_SkyboxRadius + 10);
            camera.SetDistance(2);
            camera.SetYaw(MathHelper.PiOver2);
            var controller =
                new PassthroughController(
                    new SubjectiveCamera3dController(camera, 1f)
                    {
                        KeySensitivity = 0.0005f,
                        MouseSensitivity = MathHelper.Pi,
                        MouseWheelSensitivity = 0.1f,
                        PitchRange = new(-MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f),
                        DistanceRange = new(1.1f, 10)
                    });
            float logDistance = MathF.Log(stellarBody.Orbit.GetAverageDistance() + 1);
            _skyBox ??= CreateSkybox();
            return new StellarBodyScene(
                controller, 
                camera, 
                model,
                StellarBodyViewFactory.SurfaceShader,
                StellarBodyViewFactory.AtmosphereShader,
                s_LuminanceRange.Clamp(s_LightPower * MathF.Log(stellarBody.Orbit.Focus.Luminosity + 1)),
                logDistance * logDistance / (1000 * 1000),
                StarViewFactory.CreateView(
                    Enumerable.Repeat(stellarBody.Orbit.Focus, 1), 
                    Enumerable.Repeat(s_StellarBodySceneStarPosition, 1), 
                    s_StellarBodySceneStarScale / (MathF.Log(stellarBody.Orbit.MajorAxis) + 1)),
                _skyBox);
        }

        private Skybox CreateSkybox()
        {
            var uvSphereSolid = Solid<Spherical3>.GenerateSphericalUvSphere(s_SkyboxRadius, s_SkyboxPrecision);
            Vertex3[] vertices = new Vertex3[6 * uvSphereSolid.Faces.Length];
            var projection = new CylindricalProjection.Spherical();
            for (int i = 0; i < uvSphereSolid.Faces.Length; ++i)
            {
                for (int j = 0; j < uvSphereSolid.Faces[i].Vertices.Length; ++j)
                {
                    var vert = uvSphereSolid.Faces[i].Vertices[j];
                    var texCoords = s_SkyboxResolution * new Vector2(0.5f, 1) * projection.Project(vert);
                    var c = vert.AsCartesian();
                    vertices[6 * i + j] = new(c, Color4.White, texCoords);
                }
            }

            using var canvases = new CachingCanvasProvider(new(s_SkyboxResolution, s_SkyboxResolution), Color4.Black);
            var pipeline =
                new Pipeline.Builder()
                    .AddNode(new GeneratorNode.Builder().SetKey("new"))
                    .AddNode(
                        new GradientNode.Builder()
                            .SetKey("gradient")
                            .SetChannel(Channel.Red | Channel.Green)
                            .SetInput("input", "new")
                            .SetParameters(
                                new GradientNode.Parameters()
                                {
                                    Scale = ConstantSupplier<Vector2>.Create(
                                        new Vector2(1f / s_SkyboxResolution, 1f / s_SkyboxResolution)),
                                    Gradient = ConstantSupplier<Matrix4x2>.Create(
                                        new Matrix4x2(new(1, 0), new(0, 1), new(), new()))
                                }))
                    .AddNode(
                        new SpherizeNode.Builder()
                            .SetKey("spherize")
                            .SetChannel(Channel.All)
                            .SetInput("input", "gradient"))
                    .AddNode(
                        new SpotNoiseNode.Builder()
                            .SetKey("spot-noise")
                            .SetChannel(Channel.Color)
                            .SetInput("input", "spherize")
                            .SetParameters(
                                new()
                                {
                                    Seed = ConstantSupplier<int>.Create(GetHashCode()),
                                    Frequency = ConstantSupplier<float>.Create(50f),
                                    Scale = ConstantSupplier<Interval>.Create(new Interval(0.09f, 0.11f))
                                }))
                    .AddOutput("spot-noise")
                    .Build();
            var output = pipeline.Run(canvases)[0].GetTexture();

            return new(new(vertices, PrimitiveType.Triangles), output, SkyboxShader);
        }
    }
}
