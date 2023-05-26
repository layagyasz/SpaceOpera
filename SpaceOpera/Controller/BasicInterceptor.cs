using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SpaceOpera.Controller
{
    public class BasicInterceptor<TIn, TOut> : IValueInterceptor<TOut>
    {
        public EventHandler<EventArgs>? Intercepted { get; set; }

        private readonly Func<TIn, TOut> _mapFn;

        private TOut? _value;

        public BasicInterceptor(Func<TIn, TOut> mapFn)
        {
            _mapFn = mapFn;
        }

        public TOut Get()
        {
            return _value!;
        }

        public bool Intercept(UiInteractionEventArgs interaction)
        {
            if (interaction.Button == MouseButton.Right && interaction.GetOnlyObject() is TIn)
            {
                _value = _mapFn((TIn)interaction.GetOnlyObject()!);
                Intercepted?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
        }
    }
}
