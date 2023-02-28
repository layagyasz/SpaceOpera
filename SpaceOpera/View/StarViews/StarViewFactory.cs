using Cardamom.Graphics;
using Cardamom.Mathematics;
using Cardamom.Mathematics.Color;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Spectra;

namespace SpaceOpera.View.StarViews
{
    public class StarViewFactory
    {
        private static readonly float s_InvLog2 = 0.5f / MathF.Log(2);
        private static readonly Interval s_ScaleBounds = new(0.1f, 2f);

        public RenderShader StarShader { get; }
        public SpectrumSensitivity HumanEyeSensitivity { get; }

        private readonly Random _random = new();

        public StarViewFactory(RenderShader starShader, SpectrumSensitivity humanEyeSensitivity)
        {
            StarShader = starShader;
            HumanEyeSensitivity = humanEyeSensitivity;
        }

        public StarBuffer CreateView(
            IEnumerable<Star> stars, IEnumerable<Vector3> positions, float scale, bool depthTest)
        {
            var vertices = new VertexPseudo3[stars.Count()];
            var starEnumerator = stars.GetEnumerator();
            var positionEnumerator = positions.GetEnumerator();
            int i = 0;
            while(starEnumerator.MoveNext() && positionEnumerator.MoveNext())
            {
                vertices[i++] = CreateVertex(starEnumerator.Current, positionEnumerator.Current, scale);
            }
            return new StarBuffer(vertices, StarShader, depthTest);
        }

        private VertexPseudo3 CreateVertex(Star star, Vector3 position, float scale)
        {
            var color = 
                ColorSystem.Ntsc.Transform(HumanEyeSensitivity.GetColor(new BlackbodySpectrum(star.Temperature)));
            color.A = _random.NextSingle();
            float relativeScale = 
                s_ScaleBounds.Clamp(s_InvLog2 * MathF.Log(star.Radius / Constants.SolarRadius + 1));
            return new (position, color, new(scale * relativeScale, scale * relativeScale));
        }
    }
}
