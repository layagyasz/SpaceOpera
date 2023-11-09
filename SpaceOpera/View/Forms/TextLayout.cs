using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Forms
{
    public class TextLayout : IFieldLayout
    {
        public enum TextType
        {
            Header1,
            Paragraph
        }

        public class Builder : IFieldLayout.IBuilder
        {
            private readonly FormLayout.Builder _root;

            private string _name = string.Empty;
            private TextType _type = TextType.Paragraph; 
            private string _text = string.Empty;
            private bool _supportsLineBreaks;

            internal Builder(FormLayout.Builder root)
            {
                _root = root;
            }

            public Builder SetName(string name)
            {
                _name = name;
                return this;
            }

            public Builder SetText(string text)
            {
                _text = text;
                return this;
            }

            public Builder SetTextType(TextType type)
            {
                _type = type;
                return this;
            }

            public Builder SupportsLineBreaks()
            {
                _supportsLineBreaks = true;
                return this;
            }

            public FormLayout.Builder Complete()
            {
                return _root;
            }

            public IFieldLayout Build()
            {
                return new TextLayout(_name, _type, _text, _supportsLineBreaks);
            }
        }

        public string Id => string.Empty;
        public string Name { get; }

        private readonly TextType _type;
        private readonly string _text;
        private readonly bool _supportLineBreaks;

        private TextLayout(string name, TextType type, string text, bool supportLineBreaks)
        {
            Name = name;
            _type = type;
            _text = text;
            _supportLineBreaks = supportLineBreaks;
        }

        public IUiElement Create(Form.Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return new TextUiElement(
                uiElementFactory.GetClass(GetClass(_type, style)), new InlayController(), _text, _supportLineBreaks);
        }

        private static string GetClass(TextType type, Form.Style style)
        {
            return type switch
            {
                TextType.Header1 => style.Header1!,
                TextType.Paragraph => style.Paragraph!,
                _ => throw new ArgumentException($"Unsupported TextType: {type}"),
            };
        }
    }
}
