using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Forms
{
    public class TextLayout : IFieldLayout
    {
        public class Builder : IFieldLayout.IBuilder
        {
            private readonly FormLayout.Builder _root;
            
            private string _text = string.Empty;
            private bool _supportsLineBreaks;

            internal Builder(FormLayout.Builder root)
            {
                _root = root;
            }

            public Builder SetText(string text)
            {
                _text = text;
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
                return new TextLayout(_text, _supportsLineBreaks);
            }
        }

        public string Id => string.Empty;
        public string Name => string.Empty;

        private readonly string _text;
        private readonly bool _supportLineBreaks;

        private TextLayout(string text, bool supportLineBreaks)
        {
            _text = text;
            _supportLineBreaks = supportLineBreaks;
        }

        public IUiElement CreateField(Form.Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return new TextUiElement(
                uiElementFactory.GetClass(style.Paragraph!), new InlayController(), _text, _supportLineBreaks);
        }
    }
}
