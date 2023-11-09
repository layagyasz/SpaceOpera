using Cardamom.Ui.Controller.Element;
using Cardamom.Ui;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Forms
{
    public class IconLayout : IFieldLayout
    {
        public enum IconType
        {
            Title
        }

        public class Builder : IFieldLayout.IBuilder
        {
            private readonly FormLayout.Builder _root;

            private string _name = string.Empty;
            private IconType _type;
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

            public Builder SetIconText(IconType type)
            {
                _type = type;
                return this;
            }

            public Builder SetName(string name)
            {
                _name = name;
                return this;
            }

            public FormLayout.Builder Complete()
            {
                return _root;
            }

            public IFieldLayout Build()
            {
                return new IconLayout(_name, _type, _object!);
            }
        }

        public string Id => string.Empty;
        public string Name { get; }

        private readonly IconType _type;
        private readonly object _object;

        private IconLayout(string name, IconType type, object @object)
        {
            Name = name;
            _type = type;
            _object = @object;
        }

        public IUiElement CreateField(Form.Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return iconFactory.Create(
                uiElementFactory.GetClass(GetClass(_type, style)),
                new InlayController(), 
                _object, 
                GetResolution(_type));
        }

        private static string GetClass(IconType type, Form.Style style)
        {
            return type switch
            {
                IconType.Title => style.IconTitle!,
                _ => throw new ArgumentException($"Unsupported IconType: {type}"),
            };
        }

        private static IconResolution GetResolution(IconType type)
        {
            return type switch
            {
                IconType.Title => IconResolution.High,
                _ => IconResolution.Low,
            };
        }
    }
}
