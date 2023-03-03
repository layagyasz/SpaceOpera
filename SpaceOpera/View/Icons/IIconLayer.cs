using Cardamom.Graphics;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Icons
{
    public interface IIconLayer : IRenderable
    {
        void SetAlpha(float alpha);
        void SetSize(Vector2 size);
    }
}
