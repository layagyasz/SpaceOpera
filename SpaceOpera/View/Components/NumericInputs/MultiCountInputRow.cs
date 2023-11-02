using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components.NumericInputs;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Components.NumericInputs
{
    public class MultiCountInputRow<T> : UiCompoundComponent, IKeyedUiElement<T> where T : notnull
    { 
        public EventHandler<EventArgs>? Refreshed { get; set; }

        public T Key { get; }
        public IUiElement Info { get; }
        public NumericInput NumericInput { get; }

        protected MultiCountInputRow(
            Class @class,
            T key,
            IController controller,
            IUiContainer info,
            NumericInput numericInput)
            : base(
                  controller,
                  new UiSerialContainer(@class, new ButtonController(), UiSerialContainer.Orientation.Horizontal))
        {
            Key = key;
            Info = info;
            NumericInput = numericInput;
            Add(info);
            Add(numericInput);
        }

        public void Refresh()
        {
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public static MultiCountInputRow<T> CreateAuto(
            T key,
            string name,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory,
            MultiCountInputStyles.MultiCountInputRowStyle style,
            AutoMultiCountInput<T>.IRowConfiguration configuration)
        {
            return new(
                uiElementFactory.GetClass(style.Container!),
                key,
                new AutoMultiCountInputRowController<T>(key, configuration),
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.Info!),
                    new ButtonController(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    iconFactory.Create(uiElementFactory.GetClass(style.Icon!), new InlayController(), key),
                    new TextUiElement(uiElementFactory.GetClass(style.Text!), new InlayController(), name),
                },
                NumericInput.Create(uiElementFactory, style.NumericInput!));
        }
    }
}
