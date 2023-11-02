using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Components.NumericInputs
{
    public abstract class BaseMultiCountInput<T> : DynamicUiCompoundComponent where T : notnull
    {
        public DynamicUiCompoundComponent Table { get; }
        public TextUiElement Total { get; }

        protected readonly MultiCountInputStyles.MultiCountInputStyle _style;
        protected readonly UiElementFactory _uiElementFactory;
        protected readonly IconFactory _iconFactory;

        private readonly DynamicKeyedTable<T, MultiCountInputRow<T>> _table;

        public BaseMultiCountInput(
            IController controller,
            MultiCountInputStyles.MultiCountInputStyle style,
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
                new DynamicKeyedTable<T, MultiCountInputRow<T>>(
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

        public bool TryGetRow(T key, out MultiCountInputRow<T>? row)
        {
            return _table.TryGetRow(key, out row);
        }

        protected abstract IEnumerable<T> GetKeys();
        protected abstract MultiCountInputRow<T> CreateRow(T key);
    }
}
