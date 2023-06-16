using OpenTK.Mathematics;

namespace SpaceOpera.View.Game.Common
{
    public struct Light
    {
        public Vector3 Position { get; set; }
        public Color4 Color { get; set; }
        public float Luminance { get; set; }
        public float Attenuation { get; set; }

        public Light(Vector3 position, Color4 color, float luminance, float attenuation)
        {
            Position = position;
            Color = color;
            Luminance = luminance;
            Attenuation = attenuation;
        }
    }
}
