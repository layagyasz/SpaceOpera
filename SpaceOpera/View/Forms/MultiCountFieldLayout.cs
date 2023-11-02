using Cardamom.Ui;
using SpaceOpera.View.Components.NumericInputs;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Forms
{
    public class MultiCountFieldLayout : IFieldLayout
    {
        public class Builder : IFieldLayout.IBuilder
        {
            private readonly FormLayout.Builder _root;
            private string? _id;
            private string _name = string.Empty;
            private Func<object, string>? _nameFn;
            private readonly List<object> _options = new();

            internal Builder(FormLayout.Builder root)
            {
                _root = root;
            }

            public Builder AddOption(object value)
            {
                _options.Add(value);
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

            public Builder SetOptionNameFn(Func<object, string> nameFn)
            {
                _nameFn = nameFn;
                return this;
            }

            public FormLayout.Builder Complete()
            {
                return _root;
            }

            public IFieldLayout Build()
            {
                return new MultiCountFieldLayout(_id!, _name, _nameFn!, _options);
            }
        }

        public string Id { get; }
        public string Name { get; }

        private readonly Func<object, string> _nameFn;
        private readonly List<object> _options;

        private MultiCountFieldLayout(string id, string name, Func<object, string> nameFn, IEnumerable<object> options)
        {
            Id = id;
            Name = name;
            _nameFn = nameFn;
            _options = options.ToList();
        }

        public IUiElement CreateField(Form.Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            var table = 
                new ManualMultiCountInput<object>(
                    style.MultiCount!,
                    _nameFn,
                    uiElementFactory,
                    iconFactory,
                    Comparer<object>.Create((x, y) => _nameFn(x).CompareTo(_nameFn(y))));
            table.SetOptions(_options); 
            return table;
        }
    }
}
