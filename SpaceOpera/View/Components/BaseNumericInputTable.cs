using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Components
{
    public abstract class BaseNumericInputTable<T> : DynamicUiCompoundComponent where T : notnull
    {
        public class Style
        {
            public string? Container { get; set; }
            public string? Table { get; set; }
            public NumericInputTableRow<T>.Style? Row { get; set; }
            public string? TotalContainer { get; set; }
            public string? TotalText { get; set; }
            public string? TotalNumber { get; set; }
        }

        public DynamicUiCompoundComponent Table { get; }
        public TextUiElement Total { get; }

        protected readonly Style _style;
        protected readonly UiElementFactory _uiElementFactory;
        protected readonly IconFactory _iconFactory;

        private readonly DynamicKeyedTable<T, NumericInputTableRow<T>> _table;

        public BaseNumericInputTable(
            IController controller,
            Style style,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory,
            IComparer<T> comparer)
            : base(
                  controller,
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(style.Container!),
                      new NoOpElementController(),
                      UiSerialContainer.Orientation.Vertical))
        {
            _style = style;
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            _table =
                new DynamicKeyedTable<T, NumericInputTableRow<T>>(
                        uiElementFactory.GetClass(style.Table!),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        GetKeys,
                        CreateRow,
                        comparer);
            Table = new(new RadioController<T>(), _table);
            Add(Table);

            Total =
                new TextUiElement(uiElementFactory.GetClass(style.TotalNumber!), new ButtonController(), string.Empty);
            Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.TotalContainer!),
                    new ButtonController(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    new TextUiElement(uiElementFactory.GetClass(style.TotalText!), new ButtonController(), "Total"),
                    Total
                });
        }

        public bool TryGetRow(T key, out NumericInputTableRow<T>? row)
        {
            return _table.TryGetRow(key, out row);
        }

        protected abstract IEnumerable<T> GetKeys();
        protected abstract NumericInputTableRow<T> CreateRow(T key);
    }
}
