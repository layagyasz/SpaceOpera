using Cardamom;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Icons
{
    public class IconAtom : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public Color4 Color { get; set; }
        public string Texture { get; set; } = string.Empty;

        public IconLayer ToDefinition()
        {
            return new(Color, Texture);
        }
    }
}
