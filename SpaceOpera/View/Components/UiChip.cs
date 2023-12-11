using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Components
{
    public class UiChip<T> : DynamicUiSerialContainer, IKeyedUiElement<T> where T : notnull
    {
        public class Factory : IKeyedElementFactory<T>
        {
            private readonly NameMapper<T> _nameMapper;
            private readonly ChipSetStyles.ChipStyle _style;
            private readonly UiElementFactory _uiElementFactory;
            private readonly IconFactory _iconFactory;

            public Factory(
                NameMapper<T> nameMapper,
                ChipSetStyles.ChipStyle style, 
                UiElementFactory uiElementFactory, 
                IconFactory iconFactory)
            {
                _nameMapper = nameMapper;
                _style = style;
                _uiElementFactory = uiElementFactory;
                _iconFactory = iconFactory;
            }

            public IKeyedUiElement<T> Create(T key)
            {
                return UiChip<T>.Create(key, _nameMapper(key), _style, _uiElementFactory, _iconFactory);
            }
        }

        public T Key { get; }

        private UiChip(Class @class, T key, IUiElement icon, TextUiElement text)
            : base(@class, new ButtonController(), Orientation.Horizontal)
        {
            Key = key;
            Add(icon);
            Add(text);
        }

        public static UiChip<T> Create(
            T key,
            string text,
            ChipSetStyles.ChipStyle style,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory)
        {
            return new(
                uiElementFactory.GetClass(style.Container!),
                key, 
                iconFactory.Create(uiElementFactory.GetClass(style.Icon!), new InlayController(), key), 
                new TextUiElement(uiElementFactory.GetClass(style.Text!), new InlayController(), text));
        }
    }
}
