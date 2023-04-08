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

        public EventHandler<EventArgs>? Refreshed { get; set; }

        public T Key { get; }
        public IUiElement Info { get; }
        public NumericInput NumericInput { get; }

        private NumericInputTableRow(
            Class @class, 
            T key,
            NumericInputTable<T>.IConfiguration configuration,
            IUiContainer info, 
            NumericInput numericInput)
            : base(
                  new NumericInputTableRowController<T>(key, configuration),
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

        public static NumericInputTableRow<T> Create(
            T key, 
            string name, 
            UiElementFactory uiElementFactory, 
            IconFactory iconFactory,
            Style style, 
            NumericInputTable<T>.IConfiguration configuration)
        {
            return new(
                uiElementFactory.GetClass(style.Container),
                key, 
                configuration,
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.Info), 
                    new ButtonController(), 
                    UiSerialContainer.Orientation.Horizontal)
                {
                    iconFactory.Create(uiElementFactory.GetClass(style.Icon), new InlayController(), key),
                    new TextUiElement(uiElementFactory.GetClass(style.Text), new InlayController(), name),
                }, 
                NumericInput.Create(key, uiElementFactory, style.NumericInput));
        }
    }
}
