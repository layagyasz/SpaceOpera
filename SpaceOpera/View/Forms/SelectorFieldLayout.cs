using Cardamom.Ui;
using Cardamom.Ui.Controller;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Forms
{
    public class SelectorFieldLayout : IFieldLayout
    {
        public enum SelectorType
        {
            Dial,
            DropDown,
            Radio
        }

        public class Builder : IFieldLayout.IBuilder
        {
            private readonly FormLayout.Builder _root;
            private string? _id;
            private string _name = string.Empty;
            private readonly List<SelectOption<object>> _options = new();

            private SelectorType _type;

            internal Builder(FormLayout.Builder root)
            {
                _root = root;
            }

            public Builder AddOption(string name, object value)
            {
                _options.Add(SelectOption<object>.Create(value, name));
                return this;
            }

            public Builder SetId(string id)
            {
                _id = id;
                return this;
            }

            public Builder SetName(string name)
            {
                _name = name;
                return this;
            }

            public Builder SetSelectorType(SelectorType type)
            {
                _type = type;
                return this;
            }

            public FormLayout.Builder Complete()
            {
                return _root;
            }

            public IFieldLayout Build()
            {
                return new SelectorFieldLayout(_id!, _name, _type, _options);
            }
        }

        public string Id { get; }
        public string Name { get; }

        private readonly SelectorType _type;
        private readonly List<SelectOption<object>> _options;

        private SelectorFieldLayout(
            string id, string name, SelectorType type, IEnumerable<SelectOption<object>> options)
        {
            Id = id;
            Name = name;
            _type = type;
            _options = options.ToList();
        }

        public IUiElement CreateField(Form.Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return _type switch
            {
                SelectorType.Dial =>
                    DialSelect.Create(uiElementFactory, style.Dial!, _options, _options.First()),
                SelectorType.DropDown =>
                    uiElementFactory.CreateSelect(style.DropDown!, _options, scrollSpeed: 10f).Item1,
                SelectorType.Radio =>
                    uiElementFactory.CreateRadio(style.Radio!, _options).Item1,
                _ => throw new InvalidProgramException($"Unsupported SelectorType {_type}"),
            };
        }
    }
}
