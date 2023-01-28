using Cardamom;
using System.Text.Json.Serialization;
using OpenTK.Mathematics;
using Cardamom.Mathematics.Color;

namespace SpaceOpera.Views.StellarBodyViews
{
    public class BiomeRenderDetails : IKeyed
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum BiomeRenderDetailsColorMode
        {
            Static,
            SolarPeakOutput
        }

        public struct ColorTransform
        {
            public float Hue { get; set; }
            public float Saturation { get; set; }
            public float Lightness { get; set; }

            public Color4 Apply(Color4 color)
            {
                var hsl = color.AsHsl();
                hsl.H = (hsl.H + 1 + Hue) % 1;
                hsl.S *= Saturation;
                hsl.L *= Lightness;
                return hsl.AsRgb();
            }
        }


        public string Key { get; set; } = string.Empty;
        public BiomeRenderDetailsColorMode ColorMode { get; set; } = BiomeRenderDetailsColorMode.Static;
        public Color4 Color { get; set; }
        public ColorTransform SolarPeakOutputTransform { get; set; }
        public float SpecularCoefficient { get; set; }
        public float SpecularFactor { get; set; }
        public float Luminance { get; set; }

        public Color4 GetColor(Color4 solarPeakOutputColor)
        {
            switch (ColorMode)
            {
                case BiomeRenderDetailsColorMode.Static:
                    return Color;
                case BiomeRenderDetailsColorMode.SolarPeakOutput:
                    return SolarPeakOutputTransform.Apply(solarPeakOutputColor);
                default:
                    throw new ArgumentException(string.Format("Unsupported ColorMode: {0}", ColorMode));
            }
        }
    }
}
