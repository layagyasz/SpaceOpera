using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components.NumericInputs;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Components.NumericInputs
{
    public class ManualMultiCountInput<T> : BaseMultiCountInput<T> where T : notnull
    {
        public Select Select { get; }
        public IUiElement AddButton { get; }

        new private readonly MultiCountInputStyles.ManualMultiCountInputStyle _style;
        private readonly Func<T, string> _nameFn;
        private readonly HashSet<T> _range = new();

        public ManualMultiCountInput(
            MultiCountInputStyles.ManualMultiCountInputStyle style,
            Func<T, string> nameFn,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory,
            IComparer<T> comparer)
            : base(
                  new ManualMultiCountInputController<T>(),
                  style,
                  uiElementFactory,
                  iconFactory,
                  comparer)
        {
            _style = style;
            _nameFn = nameFn;
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

        public void Add(T key)
        {
            _range.Add(key);
            Refresh();
        }

        public void SetOptions(IEnumerable<T> options)
        {
            ((SelectController<T>)Select.ComponentController)
                .SetRange(options.Select(x => SelectOption<T>.Create(x, _nameFn(x))));
        }

        public void Remove(T key)
        {
            _range.Remove(key);
            Refresh();
        }

        public void SetRange(IEnumerable<T> range)
        {
            _range.Clear();
            foreach (var item in range)
            {
                _range.Add(item);
            }
            Refresh();
        }

        protected override IEnumerable<T> GetKeys()
        {
            return _range;
        }

        protected override MultiCountInputRow<T> CreateRow(T key)
        {
            return ManualMultiCountRow<T>.Create(
                key, 
                _nameFn(key), 
                _uiElementFactory,
                _iconFactory, 
                (MultiCountInputStyles.ManualMultiCountInputRowStyle)_style.Row!);
        }
    }
}
