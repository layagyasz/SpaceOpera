using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Forms;

namespace SpaceOpera.View.Forms
{
    public class FormLayout
    {
        public class Builder
        {
            private readonly Dictionary<string, object> _hiddens = new();
            private readonly List<IFieldLayout.IBuilder> _fields = new();
            private bool _autoSubmit;

            public SelectorFieldLayout.Builder AddSelector(SelectorFieldLayout.SelectorType selectorType)
            {
                var layout = new SelectorFieldLayout.Builder(this).SetSelectorType(selectorType);
                _fields.Add(layout);
                return layout;
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

            public SelectorFieldLayout.Builder AddRadio()
            {
                return AddSelector(SelectorFieldLayout.SelectorType.Radio);
            }

            public Builder AutoSubmit()
            {
                _autoSubmit = true;
                return this;
            }

            public FormLayout Build()
            {
                return new(_hiddens, _fields.Select(x => x.Build()).ToList(), _autoSubmit);
            }
        }

        private readonly Dictionary<string, object> _hiddens;
        private readonly List<IFieldLayout> _fields;
        private readonly bool _autoSubmit;

        private FormLayout(Dictionary<string, object> hiddens, List<IFieldLayout> fields, bool autoSubmit)
        {
            _hiddens = hiddens;
            _fields = fields;
            _autoSubmit = autoSubmit;
        }

        public Form CreateForm(Form.Style style, UiElementFactory uiElementFactory)
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

                var f = field.CreateField(style, uiElementFactory);
                container.Add(f);
                if (f is IUiComponent)
                {
                    fields.Add(field.Id, f);
                }
            }
            return new(new GenericFormController(_hiddens), container, fields, _autoSubmit);
        }
    }
}
