using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller;
using SpaceOpera.Controller.Components;
using SpaceOpera.View.Components.Dynamics;

namespace SpaceOpera.View.Components
{
    public class InterceptorMultiSelect<T> : DynamicUiCompoundComponent where T : notnull
    {
        public class Style
        {
            public string? Container { get; set; }
            public string? Table { get; set; }
            public ActionRow<T>.Style? Row { get; set; }
            public string? Adder { get; set; }
        }

        public UiCompoundComponent Table { get; }
        public IUiElement Adder { get; }

        private readonly StaticRange<T> _range = new();

        public InterceptorMultiSelect(
            Style style,
            IKeyedElementFactory<T> elementFactory,
            Func<IValueInterceptor<T>> interceptorFn,
            UiElementFactory uiElementFactory,
            IComparer<T> comparer)
            : base(
                  new InterceptorMultiSelectController<T>(), 
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(style.Container!), 
                      new NoOpElementController(), 
                      UiSerialContainer.Orientation.Vertical))
        {
            Table = 
                new DynamicUiCompoundComponent(
                    new ActionComponentController(), 
                    DynamicKeyedContainer<T>.CreateSerial(
                        uiElementFactory.GetClass(style.Table!),
                        new NoOpElementController(),
                        UiSerialContainer.Orientation.Vertical,
                        _range, 
                        elementFactory, 
                        comparer));
            Add(Table);

            Adder = 
                new TextUiElement(
                    uiElementFactory.GetClass(style.Adder!), 
                    new InterceptorAdderController<T>(interceptorFn),
                    "Right click to add");
            Add(Adder);
        }

        public void Add(T key)
        {
            _range.Add(key);
            Refresh();
        }

        public void Remove(T key)
        {
            _range.Remove(key);
            Refresh();
        }

        public void SetRange(IEnumerable<T> range)
        {
            _range.Set(range);
            Refresh();
        }
    }
}
