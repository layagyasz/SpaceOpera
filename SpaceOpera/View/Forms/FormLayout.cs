using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Forms;
using System.Collections.Immutable;

namespace SpaceOpera.View.Forms
{
    public class FormLayout
    {
        private record class FieldDefinition(string Id, string Name, IFieldLayout Layout);

        public class Builder
        {
            private readonly List<FieldDefinition> _fields = new();

            public Builder Add(string id, string name, IFieldLayout field)
            {
                _fields.Add(new(id,name, field));
                return this;
            }

            public FormLayout Build()
            {
                return new(_fields);
            }
        }

        private readonly ImmutableList<FieldDefinition> _fields;

        private FormLayout(IEnumerable<FieldDefinition> fields)
        {
            _fields = fields.ToImmutableList();
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

                var f = field.Layout.CreateField(style, uiElementFactory);
                container.Add(f);
                if (f is IUiComponent)
                {
                    fields.Add(field.Id, f);
                }
            }
            return new(new GenericFormController(), container, fields);
        }
    }
}
