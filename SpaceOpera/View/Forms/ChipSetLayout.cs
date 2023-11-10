using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Forms
{
    public class ChipSetLayout<T> : IFieldLayout where T : notnull
    {
        public class Builder : IFieldLayout.IBuilder
        {
            private readonly FormLayout.Builder _root;

            private string _name = string.Empty;
            private NameMapper<T>? _nameMapper;
            private IRange<T>? _range;

            internal Builder(FormLayout.Builder root)
            {
                _root = root;
            }

            public Builder SetName(string name)
            {
                _name = name;
                return this;
            }

            public Builder SetNameMapper(NameMapper<T> nameMapper)
            {
                _nameMapper = nameMapper;
                return this;
            }

            public Builder SetRange(IRange<T> range)
            {
                _range = range;
                return this;
            }

            public FormLayout.Builder Complete()
            {
                return _root;
            }

            public IFieldLayout Build()
            {
                return new ChipSetLayout<T>(_name, _nameMapper!, _range!);
            }
        }

        public string Id => string.Empty;
        public string Name { get; }

        private readonly NameMapper<T> _nameMapper;
        private readonly IRange<T> _range;

        private ChipSetLayout(string name, NameMapper<T> nameMapper, IRange<T> range)
        {
            Name = name;
            _nameMapper = nameMapper;
            _range = range;
        }

        public IUiElement Create(Form.Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return DynamicKeyedContainer<T>.CreateChip(
                uiElementFactory.GetClass(style.ChipSet!.Container!), 
                new NoOpElementController(),
                _range, 
                new UiChip<T>.Factory(_nameMapper, style.ChipSet!.Chip!, uiElementFactory, iconFactory),
                Comparer<T>.Create((x, y) => _nameMapper(x).CompareTo(_nameMapper(y))));
        }
    }
}
