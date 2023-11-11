using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Components.Dynamics;

namespace SpaceOpera.View.Components.NumericInputs
{
    public abstract class BaseMultiCountInput<T> : DynamicUiCompoundComponent where T : notnull
    {
        public DynamicUiCompoundComponent Table { get; }
        public TextUiElement Total { get; }

        protected readonly MultiCountInputStyles.MultiCountInputStyle _style;

        private readonly DynamicKeyedContainer<T> _table;

        public BaseMultiCountInput(
            IController controller,
            MultiCountInputStyles.MultiCountInputStyle style,
            UiElementFactory uiElementFactory,
            KeyRange<T> range,
            IKeyedElementFactory<T> elementFactory,
            IComparer<T> comparer)
            : base(
                  controller,
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(style.Container!),
                      new NoOpElementController(),
                      UiSerialContainer.Orientation.Vertical))
        {
            _style = style;

            _table =
                DynamicKeyedContainer<T>.CreateSerial(
                        uiElementFactory.GetClass(style.Table!),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        range,
                        elementFactory,
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
            return _table.TryGetRowAs(key, out row);
        }
    }
}
