using Cardamom.Ui;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Icons
{
    public interface IIconLayerDefinition
    {
        IIconLayer Create(IconShaders shaders, UiElementFactory uiElementFactory);
    }
}
