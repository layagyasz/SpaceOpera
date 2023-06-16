using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SpaceOpera.Controller.Game
{
    public class BasicInterceptor<TIn, TOut> : IValueInterceptor<TOut>
    {
        public EventHandler<EventArgs>? Intercepted { get; set; }

        private readonly Func<TIn, TOut?> _mapFn;
        private readonly Func<TOut, bool> _filterFn;

        private TOut? _value;

        public BasicInterceptor(Func<TIn, TOut?> mapFn, Func<TOut, bool> filterFn)
        {
            _mapFn = mapFn;
            _filterFn = filterFn;
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
                if (_value != null && _filterFn(_value))
                {
                    Intercepted?.Invoke(this, EventArgs.Empty);
                    return true;
                }
            }
            return false;
        }
    }
}
