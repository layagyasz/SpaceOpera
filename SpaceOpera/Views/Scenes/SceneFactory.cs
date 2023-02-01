using Cardamom.Graphics.Camera;
using Cardamom.Mathematics.Color;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Spectra;
using SpaceOpera.Views.StellarBodyViews;

namespace SpaceOpera.Views.Scenes
{
    public class SceneFactory
    {
        public StellarBodyViewFactory StellarBodyViewFactory { get; }
        public SpectrumSensitivity HumanEyeSensitivity { get; }

        public SceneFactory(StellarBodyViewFactory stellarBodyViewFactory, SpectrumSensitivity humanEyeSensitivity)
        {
            StellarBodyViewFactory = stellarBodyViewFactory;
            HumanEyeSensitivity = humanEyeSensitivity;
        }

        public IScene Create(StellarBody stellarBody)
        {
            var spectrum = new BlackbodySpectrum(stellarBody.Orbit.Focus.Temperature);
            var lightColor = ToColor(HumanEyeSensitivity.GetColor(spectrum));
            var model = StellarBodyViewFactory.GenerateModel(stellarBody);
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
