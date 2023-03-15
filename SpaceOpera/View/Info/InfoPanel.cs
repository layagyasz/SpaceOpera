using Cardamom;
using Cardamom.Trackers;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Info
{
    public class InfoPanel : UiSerialContainer
    {
        public string HeaderRowClass { get; }
        public string HeaderClass { get; }
        public string HeaderIconClass { get; }
        public string HeaderTextClass { get; }
        public string RowClass { get; }
        public string RowHeadingClass { get; }
        public string RowValueClass { get; }
        public string MaterialCellClass { get; }
        public string MaterialIconClass { get; }
        public string MaterialTextClass { get; }
        public UiElementFactory UiElementFactory { get; }
        public IconFactory IconFactory { get; }

        public InfoPanel(
            Class @class,
            string headerRowClass,
            string headerClass,
            string headerIconClass,
            string headerTextClass,
            string rowClass,
            string rowHeadingClass,
            string rowValueClass,
            string materialCellClass,
            string materialIconClass,
            string materialTextClass,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory)
            : base(@class, new TableController(), Orientation.Vertical)
        {
            HeaderRowClass = headerRowClass;
            HeaderClass = headerClass;
            HeaderIconClass = headerIconClass;
            HeaderTextClass = headerTextClass;
            RowClass = rowClass;
            RowHeadingClass = rowHeadingClass;
            RowValueClass = rowValueClass;
            MaterialCellClass = materialCellClass;
            MaterialIconClass = materialIconClass;
            MaterialTextClass = materialTextClass;
            UiElementFactory = uiElementFactory;
            IconFactory = iconFactory;
        }

        public void AddTitle(object @object, string text)
        {
            AddRow(
                HeaderRowClass,
                new List<IUiElement>()
                {
                    IconFactory.Create(UiElementFactory.GetClass(HeaderIconClass), new InlayController(), @object),
                    UiElementFactory.CreateTextButton(HeaderTextClass, text).Item1
                });
        }

        public void AddHeader(string text)
        {
            AddRow(RowClass, new List<IUiElement>()
            {
                UiElementFactory.CreateTextButton(RowHeadingClass, text).Item1,
            });
        }

        public void AddValue(string name, string value)
        {
            AddRow(RowClass, new List<IUiElement>()
            {
                UiElementFactory.CreateTextButton(RowHeadingClass, name).Item1,
                UiElementFactory.CreateTextButton(RowValueClass, value).Item1
            });
        }

        public void AddValues<T>(string name, IEnumerable<T> values, string format)
        {
            AddRow(RowClass, new List<IUiElement>()
            {
                UiElementFactory.CreateTextButton(RowHeadingClass, name).Item1,
                UiElementFactory.CreateTextButton(
                    RowValueClass, string.Join("/", values.Select(x => string.Format(format, x)))).Item1
            });
        }

        public void AddCounts<V>(string countText, IEnumerable<Count<V>> counts) where V : IKeyed
        {
            var items = new List<IUiElement>()
            {
                UiElementFactory.CreateTextButton(RowHeadingClass, countText).Item1,
            };
            foreach (var count in counts)
            {
                items.Add(UiElementFactory.CreateTableRow(
                    MaterialCellClass, 
                    new List<IUiElement>()
                    {
                        IconFactory.Create(
                            UiElementFactory.GetClass(MaterialIconClass), new InlayController(), count.Key),
                        UiElementFactory.CreateTextButton(
                            MaterialTextClass, string.Format("{0}", count.Value)).Item1
                    },
                    new InlayController()));
            }
            AddRow(RowClass, items);
        }

        public void AddQuantities<V>(string quantityText, IEnumerable<Quantity<V>> quantities) where V : IKeyed
        {
            var items = new List<IUiElement>()
            {
                UiElementFactory.CreateTextButton(RowHeadingClass, quantityText).Item1,
            };
            foreach (var quantity in quantities)
            {
                items.Add(UiElementFactory.CreateTableRow(
                    MaterialCellClass,
                    new List<IUiElement>()
                    {
                        IconFactory.Create(
                            UiElementFactory.GetClass(MaterialIconClass), new InlayController(), quantity.Key),
                        UiElementFactory.CreateTextButton(
                            MaterialTextClass, string.Format("{0}", quantity.Value)).Item1
                    },
                    new InlayController()));
            }
            AddRow(RowClass, items);
        }

        public void AddBreak()
        {
            AddRow(RowClass, Enumerable.Empty<IUiElement>());
        }

        private void AddRow(string rowClass, IEnumerable<IUiElement> cells)
        {
            var row = UiElementFactory.CreateTableRow(rowClass, cells, new InlayController());
            foreach (var cell in cells)
            {
                row.Add(cell);
            }
            Add(row);
        }
    }
}