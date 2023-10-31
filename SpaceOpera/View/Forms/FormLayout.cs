using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Forms;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Forms
{
    public class FormLayout
    {
        public class Builder
        {
            private string _title = string.Empty;
            private readonly Dictionary<string, object> _hiddens = new();
            private readonly List<IFieldLayout.IBuilder> _fields = new();
            private bool _autoSubmit;

            public T AddField<T>(T builder) where T : IFieldLayout.IBuilder
            {
                _fields.Add(builder);
                return builder;
            }

            public SelectorFieldLayout.Builder AddSelector(SelectorFieldLayout.SelectorType selectorType)
            {
                return AddField(new SelectorFieldLayout.Builder(this).SetSelectorType(selectorType));
            }

            public SelectorFieldLayout.Builder AddDropDown()
            {
                return AddSelector(SelectorFieldLayout.SelectorType.DropDown);
            }

            public SelectorFieldLayout.Builder AddDial()
            {
                return AddSelector(SelectorFieldLayout.SelectorType.Dial);
            }

            public Builder AddHidden(string id, object value)
            {
                _hiddens.Add(id, value);
                return this;
            }

            public Builder AddInfo(object @object)
            {
                _fields.Add(new InfoLayout.Builder(this).SetObject(@object));
                return this;
            }

            public TextLayout.Builder AddParagraph()
            {
                return AddField(new TextLayout.Builder(this).SupportsLineBreaks());
            }

            public SelectorFieldLayout.Builder AddRadio()
            {
                return AddSelector(SelectorFieldLayout.SelectorType.Radio);
            }

            public TextLayout.Builder AddText()
            {
                return AddField(new TextLayout.Builder(this));
            }

            public Builder AutoSubmit()
            {
                _autoSubmit = true;
                return this;
            }

            public Builder SetTitle(string title)
            {
                _title = title;
                return this;
            }

            public FormLayout Build()
            {
                return new(_title, _hiddens, _fields.Select(x => x.Build()).ToList(), _autoSubmit);
            }
        }

        private readonly string _title;
        private readonly Dictionary<string, object> _hiddens;
        private readonly List<IFieldLayout> _fields;
        private readonly bool _autoSubmit;

        private FormLayout(
            string title, Dictionary<string, object> hiddens, List<IFieldLayout> fields, bool autoSubmit)
        {
            _title = title;
            _hiddens = hiddens;
            _fields = fields;
            _autoSubmit = autoSubmit;
        }

        public Form CreateForm(Form.Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            var container =
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.Container!), 
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical);
            var fields = new Dictionary<string, IUiComponent>();
            foreach (var field in _fields)
            {
                if (field.Name.Length > 0)
                {
                    container.Add(uiElementFactory.CreateTextButton(style.FieldHeader!, field.Name).Item1);
                }

                var f = field.CreateField(style, uiElementFactory, iconFactory);
                container.Add(f);
                if (f is IUiComponent c)
                {
                    fields.Add(field.Id, c);
                }
            }
            return new(new GenericFormController(_hiddens), container, _title, fields, _autoSubmit);
        }
    }
}
