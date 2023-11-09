using Cardamom.Ui;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Forms
{
    public interface IFieldLayout
    {
        public interface IBuilder
        {
            FormLayout.Builder Complete();
            IFieldLayout Build();
        }

        string Id { get; }
        string Name { get; }

        IUiElement Create(Form.Style style, UiElementFactory uiElementFactory, IconFactory iconFactory);
    }
}
