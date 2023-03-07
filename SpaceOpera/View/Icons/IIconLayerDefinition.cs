using Cardamom.Ui;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Icons
{
    public interface IIconLayerDefinition
    {
        Color4 Color { get; }
        IIconLayer Create(IconShaders shaders, UiElementFactory uiElementFactory);
        IIconLayerDefinition WithColor(Color4 color);
    }
}
