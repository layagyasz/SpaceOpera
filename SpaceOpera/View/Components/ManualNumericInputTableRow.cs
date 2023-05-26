using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Controller.Components;
using SpaceOpera.View.Icons;
using Cardamom.Ui.Controller;

namespace SpaceOpera.View.Components
{
    public class ManualNumericInputTableRow<T> : NumericInputTableRow<T> where T : notnull
    {
        new public class Style : NumericInputTableRow<T>.Style
        {
            public string? Remove { get; set; }
        }

        public IUiElement RemoveButton { get; }

        private ManualNumericInputTableRow(
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

        public static ManualNumericInputTableRow<T> Create(
            T key,
            string name,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory,
            Style style)
        {
            return new(
                uiElementFactory.GetClass(style.Container!),
                key,
                new ManualNumericInputTableRowController<T>(key),
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.Info!),
                    new ButtonController(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    iconFactory.Create(uiElementFactory.GetClass(style.Icon!), new InlayController(), key),
                    new TextUiElement(uiElementFactory.GetClass(style.Text!), new InlayController(), name),
                },
                NumericInput.Create(key, uiElementFactory, style.NumericInput),
                uiElementFactory.CreateSimpleButton(style.Remove!).Item1);
        }
    }
}
