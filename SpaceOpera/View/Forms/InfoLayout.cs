using Cardamom.Ui;
using SpaceOpera.View.Icons;
using SpaceOpera.View.Info;

namespace SpaceOpera.View.Forms
{
    public class InfoLayout : IFieldLayout
    {
        public class Builder : IFieldLayout.IBuilder
        {
            private readonly FormLayout.Builder _root;
            private object? _object;

            internal Builder(FormLayout.Builder root)
            {
                _root = root;
            }

            public Builder SetObject(object @object)
            {
                _object = @object;
                return this;
            }

            public FormLayout.Builder Complete()
            {
                return _root;
            }

            public IFieldLayout Build()
            {
                return new InfoLayout(_object!);
            }
        }

        public string Id => string.Empty;
        public string Name => string.Empty;

        private object _object;

        private InfoLayout(object @object)
        {
            _object = @object;
        }

        public IUiElement Create(Form.Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return new InfoPanel(style.Info!, uiElementFactory, iconFactory);
        }
    }
}
