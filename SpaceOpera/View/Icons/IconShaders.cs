using Cardamom.Graphics;

namespace SpaceOpera.View.Icons
{
    public class IconShaders
    {
        public RenderShader NoTexture { get; }
        public RenderShader Texture { get; }

        public IconShaders(RenderShader noTexture, RenderShader texture)
        {
            NoTexture = noTexture;
            Texture = texture;
        }
    }
}
