using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Mathematics;
using Cardamom.Mathematics.Color;
using Cardamom.Ui;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Spectra;
using SpaceOpera.View.Game.Common;
using SpaceOpera.View.Game.StellarBodyViews;

namespace SpaceOpera.View.Icons
{
    public class StellarBodyIconFactory
    {
        private static Matrix4 s_Invert = Matrix4.CreateScale(1f, -1f, 1f);
        private static readonly float s_SceneDepth = 1110f;
        private static readonly Vector3 s_LightPosition = new(1000, 0, 0);
        private static readonly float s_LightPower = 1f;
        private static readonly Interval s_LuminanceRange = new(0.1f, float.MaxValue);

        public StellarBodyViewFactory StellarBodyViewFactory { get; }

        public StellarBodyIconFactory(StellarBodyViewFactory stellarBodyViewFactory)
        {
            StellarBodyViewFactory = stellarBodyViewFactory;
        }

        public void Rasterize(StellarBody stellarBody, IRenderTarget target, IconResolution resolution)
        {
            var model = StellarBodyViewFactory.Create(stellarBody, 1, resolution == IconResolution.High);

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
                            new BlackbodySpectrum(stellarBody.Orbit.Focus.TemperatureK))),
                    s_LuminanceRange.Clamp(s_LightPower * MathF.Log(stellarBody.Orbit.Focus.LuminosityS + 1)),
                    logDistance * logDistance / (1000 * 1000));

            target.PushModelMatrix(Matrix4.Identity);
            // Renders upside-down by default, so flip it back.
            target.PushViewMatrix(camera.GetViewMatrix() * s_Invert);
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
