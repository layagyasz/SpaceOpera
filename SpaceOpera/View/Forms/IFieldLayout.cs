using Cardamom.Ui;

namespace SpaceOpera.View.Forms
{
    public interface IFieldLayout
    {
        IUiComponent CreateField(Form.Style style, UiElementFactory uiElementFactory);
    }
}
