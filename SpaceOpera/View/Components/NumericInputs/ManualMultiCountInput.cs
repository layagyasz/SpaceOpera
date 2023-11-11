using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components.NumericInputs;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Components.NumericInputs
{
    public class ManualMultiCountInput<T> : BaseMultiCountInput<T> where T : notnull
    {
        class ElementFactory : IKeyedElementFactory<T>
        {
            private readonly MultiCountInputStyles.ManualMultiCountInputStyle _style;
            private readonly NameMapper<T> _nameMapper;
            private readonly UiElementFactory _uiElementFactory;
            private readonly IconFactory _iconFactory;

            public ElementFactory(
                MultiCountInputStyles.ManualMultiCountInputStyle style, 
                NameMapper<T> nameMapper, 
                UiElementFactory uiElementFactory,
                IconFactory iconFactory)
            {
                _style = style;
                _nameMapper = nameMapper;
                _uiElementFactory = uiElementFactory;
                _iconFactory = iconFactory;
            }

            public IKeyedUiElement<T> Create(T key)
            {
                return ManualMultiCountRow<T>.Create(
                    key,
                    _nameMapper(key),
                    _uiElementFactory,
                    _iconFactory,
                    (MultiCountInputStyles.ManualMultiCountInputRowStyle)_style.Row!);
            }
        }

        public Select Select { get; }
        public IUiElement AddButton { get; }

        private readonly NameMapper<T> _nameMapper;
        private readonly StaticRange<T> _range;

        private ManualMultiCountInput(
            MultiCountInputStyles.ManualMultiCountInputStyle style,
            UiElementFactory uiElementFactory,
            NameMapper<T> nameMapper,
            StaticRange<T> range,
            IKeyedElementFactory<T> elementFactory,
            IComparer<T> comparer)
            : base(
                  new ManualMultiCountInputController<T>(),
                  style,
                  uiElementFactory,
                  range.GetRange,
                  elementFactory,
                  comparer)
        {
            _nameMapper = nameMapper;
            _range = range;

            Select =
                (Select)uiElementFactory.CreateSelect(
                    style.Select!, Enumerable.Empty<SelectOption<T>>(), scrollSpeed: 10f).Item1;
            AddButton = uiElementFactory.CreateTextButton(style.Add!, "+").Item1;
            Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.SelectWrapper!),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    Select,
                    AddButton
                });
        }

        public static ManualMultiCountInput<T> Create(
            MultiCountInputStyles.ManualMultiCountInputStyle style,
            NameMapper<T> nameMapper,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory,
            IComparer<T> comparer)
        {
            var range = new StaticRange<T>();
            return new(
                style,
                uiElementFactory, 
                nameMapper,
                range,
                new ElementFactory(style, nameMapper, uiElementFactory, iconFactory), 
                comparer);
        }

        public void Add(T key)
        {
            _range.Add(key);
            Refresh();
        }

        public void SetOptions(IEnumerable<T> options)
        {
            ((SelectController<T>)Select.ComponentController)
                .SetRange(options.Select(x => SelectOption<T>.Create(x, _nameMapper(x))));
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
