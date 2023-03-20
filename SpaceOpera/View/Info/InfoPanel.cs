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
        public InfoPanelStyle Style { get; }
        public UiElementFactory UiElementFactory { get; }
        public IconFactory IconFactory { get; }

        public InfoPanel(
            InfoPanelStyle style,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory)
            : base(
                  uiElementFactory.GetClass(style.ContainerClass!),
                  new TableController(10f), 
                  Orientation.Vertical)
        {
            Style = style;
            UiElementFactory = uiElementFactory;
            IconFactory = iconFactory;
        }

        public void AddTitle(object @object, string text)
        {
            AddRow(
                Style.HeaderRowClass!,
                new List<IUiElement>()
                {
                    IconFactory.Create(
                        UiElementFactory.GetClass(Style.HeaderIconClass!), new InlayController(), @object),
                    UiElementFactory.CreateTextButton(Style.HeaderTextClass!, text).Item1
                });
        }

        public void AddHeader(string text)
        {
            InitializeAndAdd(UiElementFactory.CreateTextButton(Style.RowHeadingClass!, text).Item1);
        }

        public void AddValue(string name, string value)
        {
            InitializeAndAdd(UiElementFactory.CreateTextButton(Style.RowHeadingClass!, name).Item1);
            InitializeAndAdd(UiElementFactory.CreateTextButton(Style.RowValueClass!, value).Item1);
        }

        public void AddValues<T>(string name, IEnumerable<T> values, string format)
        {
            InitializeAndAdd(UiElementFactory.CreateTextButton(Style.RowHeadingClass!, name).Item1);
            foreach (var value in values)
            {
                InitializeAndAdd(
                    UiElementFactory.CreateTextButton(Style.RowValueClass!, string.Format(format, value)).Item1);
            }
        }

        public void AddCounts<V>(string countText, IEnumerable<Count<V>> counts) where V : IKeyed
        {
            InitializeAndAdd(UiElementFactory.CreateTextButton(Style.RowHeadingClass!, countText).Item1);
            foreach (var count in counts)
            {
                InitializeAndAdd(
                    UiElementFactory.CreateTableRow(
                        Style.MaterialCellClass!, 
                        new List<IUiElement>()
                        {
                            IconFactory.Create(
                                UiElementFactory.GetClass(Style.MaterialIconClass!), new InlayController(), count.Key),
                            UiElementFactory.CreateTextButton(
                                Style.MaterialTextClass!, string.Format("{0:N0}", count.Value)).Item1
                        },
                        new InlayController()));
            }
        }

        public void AddQuantities<V>(string quantityText, IEnumerable<Quantity<V>> quantities) where V : IKeyed
        {
            InitializeAndAdd(UiElementFactory.CreateTextButton(Style.RowHeadingClass!, quantityText).Item1);
            foreach (var quantity in quantities)
            {
                InitializeAndAdd(
                    UiElementFactory.CreateTableRow(
                        Style.MaterialCellClass!,
                        new List<IUiElement>()
                        {
                            IconFactory.Create(
                                UiElementFactory.GetClass(Style.MaterialIconClass!),
                                new InlayController(),
                                quantity.Key),
                            UiElementFactory.CreateTextButton(
                                Style.MaterialTextClass!, string.Format("{0:N0}", quantity.Value)).Item1
                        },
                        new InlayController()));
            }
        }

        public void AddBreak()
        {
            AddRow(Style.RowClass!, Enumerable.Empty<IUiElement>());
        }

        private void AddRow(string rowClass, IEnumerable<IUiElement> cells)
        {
            var row = UiElementFactory.CreateTableRow(rowClass, cells, new InlayController());
            row.Initialize();
            Add(row);
        }

        private void InitializeAndAdd(IUiElement element)
        {
            element.Initialize();
            Add(element);
        }
    }
}