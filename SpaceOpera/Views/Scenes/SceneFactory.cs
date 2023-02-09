using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Mathematics;
using Cardamom.Mathematics.Coordinates.Projections;
using Cardamom.Mathematics.Coordinates;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Spectra;
using SpaceOpera.Views.GalaxyViews;
using SpaceOpera.Views.StarViews;
using SpaceOpera.Views.StellarBodyViews;
using OpenTK.Graphics.OpenGL4;
using Cardamom.ImageProcessing.Pipelines.Nodes;
using Cardamom.ImageProcessing.Pipelines;
using Cardamom.Utils.Suppliers;
using Cardamom.ImageProcessing;

namespace SpaceOpera.Views.Scenes
{
    public class SceneFactory
    {
        private static readonly float s_GalaxyScale = 0.0001f;
        private static readonly float s_SkyboxRadius = 1100;
        private static readonly int s_SkyboxPrecision = 64;
        private static readonly int s_SkyboxResolution = 2048;
        private static readonly float s_StarScale = 1024;
        private static readonly Vector3 s_StarPosition = new(0, 0, -1000);
        private static readonly float s_LightPower = 1f;
        private static readonly Interval s_LuminanceRange = new(0.1f, float.MaxValue);

        private static Skybox? _skyBox;

        public GalaxyViewFactory GalaxyViewFactory { get; }
        public StellarBodyViewFactory StellarBodyViewFactory { get; }
        public StarViewFactory StarViewFactory { get; }
        public SpectrumSensitivity HumanEyeSensitivity { get; }
        public RenderShader SkyboxShader { get; }

        public SceneFactory(
            GalaxyViewFactory galaxyViewFactory,
            StellarBodyViewFactory stellarBodyViewFactory,
            StarViewFactory starViewFactory,
            SpectrumSensitivity humanEyeSensitivity,
            RenderShader skyboxShader)
        {
            GalaxyViewFactory = galaxyViewFactory;
            StellarBodyViewFactory = stellarBodyViewFactory;
            StarViewFactory = starViewFactory;
            HumanEyeSensitivity = humanEyeSensitivity;
            SkyboxShader = skyboxShader;
        }

        public IScene Create(Galaxy galaxy)
        {
            var model = GalaxyViewFactory.CreateModel(galaxy, s_GalaxyScale);
            var camera = new SubjectiveCamera3d(s_SkyboxRadius + 10);
            camera.OnCameraChange += (s, e) => model.Dirty();
            camera.SetDistance(0.05f);
            camera.SetPitch(-0.125f * MathHelper.Pi);
            camera.SetYaw(MathHelper.PiOver2);
            float r = s_GalaxyScale * galaxy.Radius;
            var controller =
                new PassthroughController(
                    new GalaxyCameraController(camera)
                    {
                        Radius = r,
                        KeySensitivity = 0.0004f * r,
                        MouseWheelSensitivity = 0.02f * r,
                        PitchRange = new(-MathHelper.PiOver2 + 0.01f, -0.125f * MathHelper.Pi),
                        DistanceRange = new(0.05f, r)
                    });
            _skyBox ??= CreateSkybox();
            return new GalaxyScene(controller, camera, model, _skyBox);
        }

        public IScene Create(StellarBody stellarBody)
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
                    Enumerable.Repeat(s_StarPosition, 1), 
                    s_StarScale / (MathF.Log(stellarBody.Orbit.MajorAxis) + 1)),
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
