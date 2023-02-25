using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.ImageProcessing;
using Cardamom.ImageProcessing.Pipelines.Nodes;
using Cardamom.ImageProcessing.Pipelines;
using Cardamom.Mathematics;
using Cardamom.Mathematics.Coordinates.Projections;
using Cardamom.Mathematics.Coordinates;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Utils.Suppliers;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SpaceOpera.Controller.Scenes;
using SpaceOpera.Core;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Spectra;
using SpaceOpera.View.GalaxyViews;
using SpaceOpera.View.StarViews;
using SpaceOpera.View.StellarBodyViews;
using SpaceOpera.View.StarSystemViews;
using SpaceOpera.View.Common;
using SpaceOpera.View.Common.Highlights;

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

        private static readonly float s_StarSystemBorderWidth = 0.01f;
        private static readonly float s_StarSystemScale = 2f;

        private static readonly Interval s_StellarBodyCameraZoomRange = new(1.1f, 10f);
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

        public SceneFactory(
            GalaxyViewFactory galaxyViewFactory,
            StellarBodyViewFactory stellarBodyViewFactory,
            StarSystemViewFactory starSystemViewFactory,
            StarViewFactory starViewFactory,
            SpectrumSensitivity humanEyeSensitivity,
            RenderShader skyboxShader, 
            RenderShader borderShader, 
            RenderShader fillShader)
        {
            GalaxyViewFactory = galaxyViewFactory;
            StellarBodyViewFactory = stellarBodyViewFactory;
            StarSystemViewFactory = starSystemViewFactory;
            StarViewFactory = starViewFactory;
            HumanEyeSensitivity = humanEyeSensitivity;
            SkyboxShader = skyboxShader;
            BorderShader = borderShader;
            FillShader = fillShader;
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
            var model = StarSystemViewFactory.Create(starSystem, s_StarSystemScale);

            var distances = new float[starSystem.Orbiters.Count + 1];
            var outerLimit = MathF.Log(starSystem.ViableRange.Maximum + 1);
            var transitLimit = MathF.Log(starSystem.TransitLimit + 1);
            distances[0] = MathF.Log(starSystem.ViableRange.Minimum + 1);
            distances[^1] = outerLimit;
            for (int i=0; i<starSystem.Orbiters.Count - 1; ++i)
            {
                distances[i + 1] =
                    0.5f * MathF.Log((starSystem.Orbiters[i].Orbit.GetAverageDistance() + 1)
                    * (starSystem.Orbiters[i + 1].Orbit.GetAverageDistance() + 1));
            }
            var rigs = new StarSubSystemRig[starSystem.Orbiters.Count];
            for (int i=0; i<starSystem.Orbiters.Count; ++i)
            {
                var d = MathF.Log(starSystem.Orbiters[i].Orbit.GetAverageDistance() + 1);
                rigs[i] = 
                    StarSystemViewFactory.Create(
                        starSystem.OrbitalRegions[i],
                        calendar,
                        Math.Min(distances[i + 1] - d, d - distances[i]),
                        s_StarSystemScale);
            }

            var r = s_StarSystemScale * MathF.Log(starSystem.TransitLimit + 1);
            var transitDistance = 0.5f * (outerLimit + transitLimit);
            var transitRadius = 0.5f * (transitLimit - outerLimit);
            var bounds = new Dictionary<INavigable, SpaceSubRegionBounds>();
            var interactors = new SubRegionInteractor[starSystem.Transits.Count];
            int t = 0;
            foreach (var transit in starSystem.Transits)
            {
                var position = transitDistance * new Vector3(MathF.Cos(transit.Key), 0, MathF.Sin(transit.Key));
                bounds.Add(
                    transit.Value,
                    StarSystemSubRegionBounds.ComputeBounds(
                        position,
                        transitRadius,
                        s_StarSystemScale));
                interactors[t++] =
                    new(
                        new SubRegionController(transit.Value),
                        new Disk(s_StarSystemScale * position, Vector3.UnitY, s_StarSystemScale * transitRadius));
            }

            var camera = new SubjectiveCamera3d(s_SkyboxRadius + 10);
            camera.OnCameraChange += (s, e) => model.Dirty();
            camera.SetDistance(r);
            camera.SetPitch(-MathHelper.PiOver2 + 0.01f);
            camera.SetYaw(MathHelper.PiOver2);
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
                    Enumerable.Concat(
                        interactors.Select(x => x.Controller), 
                        rigs.Select(x => x.Controller))
                    .Cast<ISceneController>().ToArray());

            _skyBox ??= CreateSkybox();
            return new StarSystemScene(
                controller,
                camera,
                model,
                StellarBodyViewFactory.SurfaceShader,
                StellarBodyViewFactory.AtmosphereShader,
                new(
                    new(), 
                    model.GetLightColor(), 
                    GetLuminance(starSystem.Star), 
                    1f / (s_StarSystemScale * s_StarSystemScale)),
                rigs,
                interactors,
                new(
                    starSystem.Transits.Values,
                    bounds,
                    s_StarSystemScale * s_StarSystemBorderWidth,
                    Matrix4.Identity,
                    BorderShader, 
                    FillShader),
                _skyBox);
        }

        public IGameScene Create(StellarBody stellarBody)
        {
            var model = StellarBodyViewFactory.Create(stellarBody, 1f, true);
            var camera = new SubjectiveCamera3d(s_SkyboxRadius + 10);
            camera.SetDistance(2 * model.Radius);
            camera.SetYaw(MathHelper.PiOver2);
            var controller =
                new SceneController(
                    new SubjectiveCamera3dController(camera, model.Radius)
                    {
                        KeySensitivity = 0.0005f,
                        MouseSensitivity = MathHelper.Pi,
                        MouseWheelSensitivity = 0.1f,
                        PitchRange = new(-MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f),
                        DistanceRange = 
                            new(
                                s_StellarBodyCameraZoomRange.Minimum * model.Radius, 
                                s_StellarBodyCameraZoomRange.Maximum * model.Radius)
                    });
            float logDistance = MathF.Log(stellarBody.Orbit.GetAverageDistance() + 1);
            var star =
                StarViewFactory.CreateView(
                    Enumerable.Repeat(stellarBody.Orbit.Focus, 1),
                    Enumerable.Repeat(s_StellarBodySceneStarPosition, 1),
                    s_StellarBodySceneStarScale / (MathF.Log(stellarBody.Orbit.MajorAxis) + 1));
            _skyBox ??= CreateSkybox();
            return new StellarBodyScene(
                controller, 
                camera, 
                model,
                StellarBodyViewFactory.SurfaceShader,
                StellarBodyViewFactory.AtmosphereShader,
                new(
                    new(), 
                    star.Get(0).Color, 
                    GetLuminance(stellarBody.Orbit.Focus),
                    logDistance * logDistance / (1000 * 1000)),
                star,
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

        private static float GetLuminance(Star star)
        {
            return s_LuminanceRange.Clamp(s_LightPower * MathF.Log(star.Luminosity + 1));
        }
    }
}
