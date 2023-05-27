
using Cardamom.Ui;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller;
using SpaceOpera.Controller.Components;

namespace SpaceOpera.View.Components
{
    public class InterceptorInput<T> : UiSerialContainer
    {
        private readonly Func<T?, IEnumerable<IUiElement>> _contentsFn;

        public InterceptorInput(
            Class @class, Func<IValueInterceptor<T>> interceptorFn, Func<T?, IEnumerable<IUiElement>> contentsFn)
            : base(
                  @class,
                  new InterceptorInputController<T>("interceptor-input", interceptorFn),
                  Orientation.Horizontal)
        {
            _contentsFn = contentsFn;
            SetValue(default);
        }

        public void SetValue(T? value)
        {
            Clear(true);
            foreach (var element in _contentsFn(value))
            {
                element.Initialize();
                Add(element);
            }
        }
    }
}
