using Cardamom.Ui;

namespace SpaceOpera.View.Forms
{
    public interface IFieldLayout
    {
        public interface IBuilder
        {
            FormLayout.Builder CompleteField();
            IFieldLayout Build();
        }

        string Id { get; }
        string Name { get; }

        IUiComponent CreateField(Form.Style style, UiElementFactory uiElementFactory);
    }
}
