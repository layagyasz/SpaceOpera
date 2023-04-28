using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Mathematics;
using Cardamom.Mathematics.Color;
using Cardamom.Ui;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Spectra;
using SpaceOpera.View.Common;
using SpaceOpera.View.StellarBodyViews;

namespace SpaceOpera.View.Icons
{
    public class StellarBodyIconFactory
    {
        private static readonly float s_SceneDepth = 1110f;
        private static readonly Vector3 s_LightPosition = new(1000, 0, 0);
        private static readonly float s_LightPower = 1f;
        private static readonly Interval s_LuminanceRange = new(0.1f, float.MaxValue);

        public StellarBodyViewFactory StellarBodyViewFactory { get; }

        public StellarBodyIconFactory(StellarBodyViewFactory stellarBodyViewFactory)
        {
            StellarBodyViewFactory = stellarBodyViewFactory;
        }

        public void Rasterize(StellarBody stellarBody, IRenderTarget target)
        {
            var model = StellarBodyViewFactory.Create(stellarBody, 1, false);

            var camera = new SubjectiveCamera3d(s_SceneDepth);
            camera.SetDistance(1.5f * model.Radius);
            camera.SetYaw(MathHelper.PiOver2);
            camera.SetAspectRatio(1);

            float logDistance = MathF.Log(stellarBody.Orbit.GetAverageDistance() + 1);
            var light =
                new Light(
                    new(),
                    ColorSystem.Ntsc.Transform(
                        StellarBodyViewFactory.HumanEyeSensitivity.GetColor(
                            new BlackbodySpectrum(stellarBody.Orbit.Focus.Temperature))),
                    s_LuminanceRange.Clamp(s_LightPower * MathF.Log(stellarBody.Orbit.Focus.Luminosity + 1)),
                    logDistance * logDistance / (1000 * 1000));

            target.PushModelMatrix(Matrix4.Identity);
            target.PushViewMatrix(camera.GetViewMatrix());
            target.PushProjection(camera.GetProjection());

            var surfaceShader = StellarBodyViewFactory.SurfaceShader;
            var atmosphereShader = StellarBodyViewFactory.AtmosphereShader;
            surfaceShader.SetVector3("light_position", s_LightPosition);
            surfaceShader.SetVector3("eye_position", camera.Position);
            surfaceShader.SetColor("light_color", light.Color);
            surfaceShader.SetFloat("light_luminance", light.Luminance);
            surfaceShader.SetFloat("light_attenuation", light.Attenuation);
            surfaceShader.SetFloat("ambient", 0.5f);
            atmosphereShader.SetVector3("light_position", s_LightPosition);
            atmosphereShader.SetVector3("eye_position", camera.Position);
            atmosphereShader.SetColor("light_color", light.Color);
            atmosphereShader.SetFloat("light_luminance", light.Luminance);
            atmosphereShader.SetFloat("light_attenuation", light.Attenuation);

            model.Draw(target, new SimpleUiContext());
            target.PopProjectionMatrix();
            target.PopViewMatrix();
            target.PopModelMatrix();
        }
    }
}
