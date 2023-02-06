using Cardamom.Graphics.Camera;
using Cardamom.Mathematics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Spectra;
using SpaceOpera.Views.GalaxyViews;
using SpaceOpera.Views.StarViews;
using SpaceOpera.Views.StellarBodyViews;

namespace SpaceOpera.Views.Scenes
{
    public class SceneFactory
    {
        private static readonly float s_GalaxyScale = 0.00005f;
        private static readonly float s_StarScale = 1024;
        private static readonly Vector3 s_StarPosition = new(0, 0, -999);
        private static readonly float s_LightPower = 0.5f;
        private static readonly Interval s_LuminanceRange = new(0.1f, float.MaxValue);

        public GalaxyViewFactory GalaxyViewFactory { get; }
        public StellarBodyViewFactory StellarBodyViewFactory { get; }
        public StarViewFactory StarViewFactory { get; }
        public SpectrumSensitivity HumanEyeSensitivity { get; }

        public SceneFactory(
            GalaxyViewFactory galaxyViewFactory,
            StellarBodyViewFactory stellarBodyViewFactory,
            StarViewFactory starViewFactory,
            SpectrumSensitivity humanEyeSensitivity)
        {
            GalaxyViewFactory = galaxyViewFactory;
            StellarBodyViewFactory = stellarBodyViewFactory;
            StarViewFactory = starViewFactory;
            HumanEyeSensitivity = humanEyeSensitivity;
        }

        public IScene Create(Galaxy galaxy)
        {
            var model = GalaxyViewFactory.CreateModel(galaxy, s_GalaxyScale);
            var camera = new SubjectiveCamera3d(1000);
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
            return new GalaxyScene(controller, camera, model);
        }

        public IScene Create(StellarBody stellarBody)
        {
            var model = StellarBodyViewFactory.CreateModel(stellarBody);
            var camera = new SubjectiveCamera3d(1000);
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
            return new StellarBodyScene(
                controller, 
                camera, 
                model,
                StellarBodyViewFactory.SurfaceShader,
                s_LuminanceRange.Clamp(s_LightPower * MathF.Log(stellarBody.Orbit.Focus.Luminosity + 1)),
                logDistance * logDistance / (1000 * 1000),
                StarViewFactory.CreateView(
                    Enumerable.Repeat(stellarBody.Orbit.Focus, 1), 
                    Enumerable.Repeat(s_StarPosition, 1), 
                    s_StarScale / (MathF.Log(stellarBody.Orbit.MajorAxis) + 1)));
        }
    }
}
