using Cardamom.Graphics;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Icons
{
    public record class IconImage(Color4 Color, Texture Texture, Box2i TextureView, bool IsDisposable);
}
