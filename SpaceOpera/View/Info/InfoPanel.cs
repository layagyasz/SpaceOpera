using Cardamom.Trackers;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Info
{
    public class InfoPanel : UiSerialContainer
    {
        public struct Style
        {
            public string? Container { get; set; }
            public string? HeaderRow { get; set; }
            public string? HeaderIcon { get; set; }
            public string? HeaderText { get; set; }
            public string? Row { get; set; }
            public string? RowHeading { get; set; }
            public string? RowValue { get; set; }
            public string? MaterialCell { get; set; }
            public string? MaterialIcon { get; set; }
            public string? MaterialText { get; set; }
        }

        private readonly Style _style;
        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        public InfoPanel(
            Style style,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory)
            : base(
                  uiElementFactory.GetClass(style.Container!),
                  new TableController(10f),
                  Orientation.Vertical)
        {
            _style = style;
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
        }

        public void AddTitle(object @object, string text)
        {
            AddRow(
                _style.HeaderRow!,
                new List<IUiElement>()
                {
                    _iconFactory.Create(
                        _uiElementFactory.GetClass(_style.HeaderIcon!), new InlayController(), @object),
                    _uiElementFactory.CreateTextButton(_style.HeaderText!, text).Item1
                });
        }

        public void AddHeader(string text)
        {
            InitializeAndAdd(_uiElementFactory.CreateTextButton(_style.RowHeading!, text).Item1);
        }

        public void AddValue(string name, string value)
        {
            InitializeAndAdd(_uiElementFactory.CreateTextButton(_style.RowHeading!, name).Item1);
            InitializeAndAdd(_uiElementFactory.CreateTextButton(_style.RowValue!, value).Item1);
        }

        public void AddValues<T>(string name, IEnumerable<T> values, string format)
        {
            InitializeAndAdd(_uiElementFactory.CreateTextButton(_style.RowHeading!, name).Item1);
            foreach (var value in values)
            {
                InitializeAndAdd(
                    _uiElementFactory.CreateTextButton(_style.RowValue!, string.Format(format, value)).Item1);
            }
        }

        public void AddCounts<T>(string countText, IEnumerable<Count<T>> counts)
        {
            InitializeAndAdd(_uiElementFactory.CreateTextButton(_style.RowHeading!, countText).Item1);
            foreach (var count in counts)
            {
                InitializeAndAdd(
                    _uiElementFactory.CreateTableRow(
                        _style.MaterialCell!,
                        new List<IUiElement>()
                        {
                            _iconFactory.Create(
                                _uiElementFactory.GetClass(_style.MaterialIcon!), new InlayController(), count.Key!),
                            _uiElementFactory.CreateTextButton(
                                _style.MaterialText!, string.Format("{0:N0}", count.Value)).Item1
                        },
                        new InlayController()));
            }
        }

        public void AddQuantities<T>(string quantityText, IEnumerable<Quantity<T>> quantities)
        {
            InitializeAndAdd(_uiElementFactory.CreateTextButton(_style.RowHeading!, quantityText).Item1);
            foreach (var quantity in quantities)
            {
                InitializeAndAdd(
                    _uiElementFactory.CreateTableRow(
                        _style.MaterialCell!,
                        new List<IUiElement>()
                        {
                            _iconFactory.Create(
                                _uiElementFactory.GetClass(_style.MaterialIcon!),
                                new InlayController(),
                                quantity.Key!),
                            _uiElementFactory.CreateTextButton(
                                _style.MaterialText!, string.Format("{0:N0}", quantity.Value)).Item1
                        },
                        new InlayController()));
            }
        }

        public void AddBreak()
        {
            AddRow(_style.Row!, Enumerable.Empty<IUiElement>());
        }

        public void SetObject(object @object)
        {

        }

        private void AddRow(string rowClass, IEnumerable<IUiElement> cells)
        {
            var row = _uiElementFactory.CreateTableRow(rowClass, cells, new InlayController());
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