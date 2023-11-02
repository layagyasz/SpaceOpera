using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Controller.Components.NumericInputs;
using SpaceOpera.View.Icons;
using Cardamom.Ui.Controller;

namespace SpaceOpera.View.Components.NumericInputs
{
    public class ManualMultiCountRow<T> : MultiCountInputRow<T> where T : notnull
    {
        public IUiElement RemoveButton { get; }

        private ManualMultiCountRow(
            Class @class,
            T key,
            IController controller,
            IUiContainer info,
            NumericInput numericInput,
            IUiElement removeButton)
            : base(@class, key, controller, info, numericInput)
        {
            RemoveButton = removeButton;
            Add(RemoveButton);
        }

        public static ManualMultiCountRow<T> Create(
            T key,
            string name,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory,
            MultiCountInputStyles.ManualMultiCountInputRowStyle style)
        {
            return new(
                uiElementFactory.GetClass(style.Container!),
                key,
                new ManualMultiCountInputRowController<T>(key),
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.Info!),
                    new ButtonController(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    iconFactory.Create(uiElementFactory.GetClass(style.Icon!), new InlayController(), key),
                    new TextUiElement(uiElementFactory.GetClass(style.Text!), new InlayController(), name),
                },
                NumericInput.Create(uiElementFactory, style.NumericInput!),
                uiElementFactory.CreateSimpleButton(style.Remove!).Item1);
        }
    }
}
