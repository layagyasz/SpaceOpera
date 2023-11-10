using Cardamom.Ui;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Components.Dynamics
{
    public class SimpleKeyedElementFactory<T> : IKeyedElementFactory<T>
    {
        public delegate IKeyedUiElement<T> Creator(T key, UiElementFactory uiElementFactory, IconFactory iconFactory);

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;
        private readonly Creator _creator;

        public SimpleKeyedElementFactory(
            UiElementFactory uiElementFactory, IconFactory iconFactory, SimpleKeyedElementFactory<T>.Creator creator)
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
            _creator = creator;
        }

        public IKeyedUiElement<T> Create(T key)
        {
            return _creator(key, _uiElementFactory, _iconFactory);
        }
    }
}
