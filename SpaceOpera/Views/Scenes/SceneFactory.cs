using Cardamom.Graphics.Camera;
using Cardamom.Mathematics;
using Cardamom.Mathematics.Color;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Spectra;
using SpaceOpera.Views.GalaxyViews;
using SpaceOpera.Views.StellarBodyViews;

namespace SpaceOpera.Views.Scenes
{
    public class SceneFactory
    {
        public GalaxyViewFactory GalaxyViewFactory { get; }
        public StellarBodyViewFactory StellarBodyViewFactory { get; }
        public SpectrumSensitivity HumanEyeSensitivity { get; }

        public SceneFactory(
            GalaxyViewFactory galaxyViewFactory,
            StellarBodyViewFactory stellarBodyViewFactory, 
            SpectrumSensitivity humanEyeSensitivity)
        {
            GalaxyViewFactory = galaxyViewFactory;
            StellarBodyViewFactory = stellarBodyViewFactory;
            HumanEyeSensitivity = humanEyeSensitivity;
        }

        public IScene Create(Galaxy galaxy)
        {
            var model = GalaxyViewFactory.CreateModel(galaxy);
            var camera = new SubjectiveCamera3d(1.5f, 1000, new(), 1);
            camera.OnCameraChange += (s, e) => model.Dirty();
            camera.SetPitch(-MathHelper.PiOver2 + 0.01f);
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
            var spectrum = new BlackbodySpectrum(stellarBody.Orbit.Focus.Temperature);
            var lightColor = ToColor(HumanEyeSensitivity.GetColor(spectrum));
            var model = StellarBodyViewFactory.CreateModel(stellarBody);
            var camera = new SubjectiveCamera3d(1.5f, 1000, new(), 2);
            var controller =
                new PassthroughController(
                    new SubjectiveCamera3dController(camera)
                    {
                        KeySensitivity = 0.0005f,
                        MouseWheelSensitivity = 0.1f,
                        PitchRange = new(-MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f),
                        DistanceRange = new(1.1f, 10)
                    });
            return new StellarBodyScene(controller, camera, model, lightColor, StellarBodyViewFactory.SurfaceShader);
        }

        private static Color4 ToColor(ColorCie color)
        {
            return ColorSystem.Ntsc.Transform(color);
        }
    }
}
