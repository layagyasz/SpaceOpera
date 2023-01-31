using Cardamom.Graphics.Camera;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.Views.StellarBodyViews;

namespace SpaceOpera.Views.Scenes
{
    public class SceneFactory
    {
        public StellarBodyViewFactory StellarBodyViewFactory { get; }

        public SceneFactory(StellarBodyViewFactory stellarBodyViewFactory)
        {
            StellarBodyViewFactory = stellarBodyViewFactory;
        }

        public IScene Create(StellarBody stellarBody)
        {
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
            return new StellarBodyScene(controller, camera, model);
        }
    }
}
