﻿using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Economics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Components
{
    public class ManualNumericInputTable<T> : BaseNumericInputTable<T> where T : notnull
    {
        new public class Style : BaseNumericInputTable<T>.Style
        {
            public string? SelectWrapper { get; set; }
            public string? Select { get; set; }
            public string? SelectDropBox { get; set; }
            public string? SelectOption { get; set; }
            public string? Add { get; set; }
        }

        public Select Select { get; }
        public IUiElement AddButton { get; }

        new private readonly Style _style;
        private readonly Func<T, string> _nameFn;
        private readonly HashSet<T> _range = new();

        public ManualNumericInputTable(
            Style style,
            Func<T, string> nameFn,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory,
            IComparer<T> comparer)
            : base(
                  new ManualNumericInputTableController<T>("manual-numeric-input-table"),
                  style,
                  uiElementFactory,
                  iconFactory,
                  comparer)
        {
            _style = style;
            _nameFn = nameFn;
            Select =
                (Select)uiElementFactory.CreateSelect<IMaterial>(
                    style.Select!, style.SelectDropBox!, Enumerable.Empty<IUiElement>(), 10f).Item1;
            AddButton = uiElementFactory.CreateTextButton(style.Add!, "+").Item1;
            Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.SelectWrapper!),
                    new NoOpElementController<UiSerialContainer>(),
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
            Select.Clear(/* dispose= */ true);
            foreach (var option in options)
            {
                var o = _uiElementFactory.CreateSelectOption(_style.SelectOption!, option, _nameFn(option)).Item1;
                o.Initialize();
                Select.Add(o);
            }
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

        protected override NumericInputTableRow<T> CreateRow(T key)
        {
            return NumericInputTableRow<T>.CreateManual(
                key, _nameFn(key), _uiElementFactory, _iconFactory, _style.Row!.Value);
        }
    }
}
