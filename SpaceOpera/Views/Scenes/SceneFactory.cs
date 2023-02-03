using Cardamom.Graphics.Camera;
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
        private static readonly float s_StarScale = 1024;
        private static readonly Vector3 s_StarPosition = new(0, 0, -999);

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
            var model = GalaxyViewFactory.CreateModel(galaxy);
            var camera = new SubjectiveCamera3d(1000);
            camera.OnCameraChange += (s, e) => model.Dirty();
            camera.SetDistance(0.05f);
            camera.SetPitch(-0.125f * MathHelper.Pi);
            camera.SetYaw(MathHelper.PiOver2);
            var controller =
                new PassthroughController(
                    new GalaxyCameraController(camera)
                    {
                        Radius2 = 1,
                        KeySensitivity = 0.0004f,
                        MouseWheelSensitivity = 0.02f,
                        PitchRange = new(-MathHelper.PiOver2 + 0.01f, -0.125f * MathHelper.Pi),
                        DistanceRange = new(0.05f, 1)
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
                    new SubjectiveCamera3dController(camera)
                    {
                        KeySensitivity = 0.0005f,
                        MouseSensitivity = MathHelper.PiOver2,
                        MouseWheelSensitivity = 0.1f,
                        PitchRange = new(-MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f),
                        DistanceRange = new(1.1f, 10)
                    });
            return new StellarBodyScene(
                controller, 
                camera, 
                model,
                StellarBodyViewFactory.SurfaceShader,
                StarViewFactory.CreateView(
                    Enumerable.Repeat(stellarBody.Orbit.Focus, 1), 
                    Enumerable.Repeat(s_StarPosition, 1), 
                    s_StarScale / (MathF.Log(stellarBody.Orbit.MajorAxis) + 1)));
        }
    }
}
