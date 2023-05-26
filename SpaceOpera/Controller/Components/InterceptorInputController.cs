using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class InterceptorInputController<T> 
        : ClassedUiElementController<InterceptorInput<T>>, IInterceptorController
    {
        public EventHandler<IInterceptor>? InterceptorCreated { get; set; }
        public EventHandler<IInterceptor>? InterceptorCancelled { get; set; }

        private readonly Func<IValueInterceptor<T>> _interceptorFn;
        private IValueInterceptor<T>? _interceptor;

        public InterceptorInputController(Func<IValueInterceptor<T>> interceptorFn)
        {
            _interceptorFn = interceptorFn;
        }

        public override bool HandleMouseButtonClicked(MouseButtonClickEventArgs e)
        {
            Clicked?.Invoke(this, e);
            return true;
        }

        public override bool HandleMouseEntered()
        {
            SetHover(true);
            return true;
        }

        public override bool HandleMouseLeft()
        {
            SetHover(false);
            return true;
        }

        public override bool HandleFocusEntered()
        {
            SetFocus(true);
            SetToggle(true);
            CreateInterceptor();
            return true;
        }

        public override bool HandleFocusLeft()
        {
            SetFocus(false);
            SetToggle(false);
            CancelInterceptor();
            return true;
        }

        private void CreateInterceptor()
        {
            CancelInterceptor();
            _interceptor = _interceptorFn();
            _interceptor.Intercepted += HandleIntercepted;
            InterceptorCreated?.Invoke(this, _interceptor);
        }

        private void CancelInterceptor()
        {
            if (_interceptor != null)
            {
                InterceptorCancelled?.Invoke(this, _interceptor);
                _interceptor.Intercepted -= HandleIntercepted;
                _interceptor = null;
            }
        }

        private void HandleIntercepted(object? sender, EventArgs e)
        {
            _element!.SetValue(_interceptor!.Get());
        }
    }
}
