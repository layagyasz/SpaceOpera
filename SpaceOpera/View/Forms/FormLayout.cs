using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Forms;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Forms
{
    public class FormLayout : IFieldLayout
    {
        public class Builder : IFieldLayout.IBuilder
        {
            private readonly Builder? _root;

            private string _name = string.Empty;
            private UiSerialContainer.Orientation _orientation = UiSerialContainer.Orientation.Vertical;
            private readonly Dictionary<string, object> _hiddens = new();
            private readonly List<IFieldLayout.IBuilder> _fields = new();
            private bool _autoSubmit;
            private string? _overrideClass;

            public Builder() { }

            private Builder(Builder root)
            {
                _root = root;
            }

            public T AddField<T>(T builder) where T : IFieldLayout.IBuilder
            {
                _fields.Add(builder);
                return builder;
            }

            public SelectorFieldLayout.Builder AddSelector(SelectorFieldLayout.SelectorType selectorType)
            {
                return AddField(new SelectorFieldLayout.Builder(this).SetSelectorType(selectorType));
            }

            public ChipSetLayout<T>.Builder AddChipSet<T>() where T : notnull
            {
                return AddField(new ChipSetLayout<T>.Builder(this));
            }

            public SelectorFieldLayout.Builder AddDropDown()
            {
                return AddSelector(SelectorFieldLayout.SelectorType.DropDown);
            }

            public SelectorFieldLayout.Builder AddDial()
            {
                return AddSelector(SelectorFieldLayout.SelectorType.Dial);
            }

            public Builder AddDiv(string @class)
            {
                return AddField(new Builder(this).SetOverrideClass(@class));
            }

            public TextLayout.Builder AddHeader1()
            {
                return AddField(new TextLayout.Builder(this).SetTextType(TextLayout.TextType.Header1));
            }

            public Builder AddHidden(string id, object value)
            {
                _hiddens.Add(id, value);
                return this;
            }

            public IconLayout.Builder AddIcon()
            {
                return AddField(new IconLayout.Builder(this));
            }

            public Builder AddInfo(object @object)
            {
                _fields.Add(new InfoLayout.Builder(this).SetObject(@object));
                return this;
            }

            public MultiCountFieldLayout.Builder AddMultiCount()
            {
                return AddField(new MultiCountFieldLayout.Builder(this));
            }

            public TextLayout.Builder AddParagraph()
            {
                return AddField(
                    new TextLayout.Builder(this).SetTextType(TextLayout.TextType.Paragraph).SupportsLineBreaks());
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

            public Builder SetName(string name)
            {
                _name = name;
                return this;
            }

            public Builder SetOrientation(UiSerialContainer.Orientation orientation)
            {
                _orientation = orientation;
                return this;
            }

            public Builder SetOverrideClass(string @class)
            {
                _overrideClass = @class;
                return this;
            }

            public Builder Complete()
            {
                return _root!;
            }

            public IFieldLayout Build()
            {
                return new FormLayout(
                    _name, 
                    _orientation,
                    _hiddens, 
                    _fields.Select(x => x.Build()).ToList(),
                    _autoSubmit, 
                    _overrideClass);
            }
        }

        public string Id => string.Empty;
        public string Name { get; }

        private readonly UiSerialContainer.Orientation _orientation;
        private readonly Dictionary<string, object> _hiddens;
        private readonly List<IFieldLayout> _fields;
        private readonly bool _autoSubmit;
        private readonly string? _overrideClass;

        private FormLayout(
            string name, 
            UiSerialContainer.Orientation orientation,
            Dictionary<string, object> hiddens,
            List<IFieldLayout> fields,
            bool autoSubmit,
            string? overrideClass)
        {
            Name = name;
            _orientation = orientation;
            _hiddens = hiddens;
            _fields = fields;
            _autoSubmit = autoSubmit;
            _overrideClass = overrideClass;
        }

        public IUiElement Create(Form.Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            var container =
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(_overrideClass ?? style.Container!), 
                    new TableController(10f),
                    _orientation);
            var fields = new Dictionary<string, IUiComponent>();
            foreach (var field in _fields)
            {
                if (field.Name.Length > 0)
                {
                    container.Add(uiElementFactory.CreateTextButton(style.Header3!, field.Name).Item1);
                }

                var f = field.Create(style, uiElementFactory, iconFactory);
                container.Add(f);
                if (field.Id.Length > 0 && f is IUiComponent c)
                {
                    fields.Add(field.Id, c);
                }
            }
            return new Form(new GenericFormController(_hiddens), container, Name, fields, _autoSubmit);
        }
    }
}
