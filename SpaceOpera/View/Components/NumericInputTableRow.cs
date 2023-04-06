using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Components
{
    public class NumericInputTableRow<T> : UiCompoundComponent, IKeyedUiElement<T> where T : notnull
    {
        public struct Style
        {
            public string Container { get; set; }
            public string Info { get; set; }
            public string Icon { get; set; }
            public string Text { get; set; }
            public NumericInput.Style NumericInput { get; set; }
        }

        public T Key { get; }
        public NumericInput NumericInput { get; }

        private NumericInputTableRow(
            Class @class, T key, IUiContainer info, NumericInput numericInput)
            : base(
                  new NumericInputTableRowController<T>(key),
                  new UiSerialContainer(@class, new ButtonController(), UiSerialContainer.Orientation.Horizontal))
        {
            Key = key;
            NumericInput = numericInput;
            Add(info);
            Add(numericInput);
        }

        public void Refresh() { }

        public static NumericInputTableRow<T> Create(
            T key, string name, UiElementFactory uiElementFactory, ref IconFactory iconFactory, Style style)
        {
            return new(
                uiElementFactory.GetClass(style.Container),
                key, 
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.Info), 
                    new ButtonController(), 
                    UiSerialContainer.Orientation.Horizontal)
                {
                    iconFactory.Create(uiElementFactory.GetClass(style.Icon), new InlayController(), key),
                    uiElementFactory.CreateTextButton(style.Text, name).Item1
                }, 
                NumericInput.Create(key, uiElementFactory, style.NumericInput));
        }
    }
}
