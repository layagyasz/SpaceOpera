using Cardamom;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Icons
{
    public class StaticIconConfig : IKeyed
    {
        public record class Layer(Color4 Color, string Texture)
        {
            public IconLayer ToDefinition()
            {
                return new(Color, Texture);
            }
        }

        public string Key { get; set; } = string.Empty;
        public List<Layer> Layers { get; set; } = new();

        public IEnumerable<IconLayer> ToDefinition()
        {
            return Layers.Select(x => x.ToDefinition());
        }
    }
}
