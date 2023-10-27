using Cardamom.Ui;

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

        IUiElement CreateField(Form.Style style, UiElementFactory uiElementFactory);
    }
}
