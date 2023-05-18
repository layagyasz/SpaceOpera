﻿using Cardamom.Mathematics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Components
{
    public class NumericInputTable<T> : DynamicUiCompoundComponent where T : notnull
    {
        public interface IRowConfiguration
        {
            string GetName(T key);
            IntInterval GetRange(T key);
            int GetValue(T key);
            IComparer<T> GetComparer();
        }

        public struct Style
        {
            public string Container { get; set; }
            public string Table { get; set; }
            public NumericInputTableRow<T>.Style Row { get; set; }
            public string TotalContainer { get; set; }
            public string TotalText { get; set; }
            public string TotalNumber { get; set; }
        }

        public DynamicUiCompoundComponent Table { get; }
        public TextUiElement Total { get; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;
        private readonly Style _style;
        private readonly IRowConfiguration _configuration;

        private readonly DynamicKeyedTable<T, NumericInputTableRow<T>> _table;

        public NumericInputTable(
            Func<IEnumerable<T>> keysFn,
            Func<IntInterval> rangeFn,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory, 
            Style style,
            IRowConfiguration configuration)
            : base(
                  new SyncingNumericInputTableController<T>(rangeFn),
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(style.Container),
                      new NoOpElementController<DynamicUiSerialContainer>(),
                      UiSerialContainer.Orientation.Vertical))
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
            _style = style;
            _configuration = configuration;

            _table =
                new DynamicKeyedTable<T, NumericInputTableRow<T>>(
                        uiElementFactory.GetClass(style.Table),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        keysFn,
                        CreateRow,
                        _configuration.GetComparer());
            Table = new(new RadioController<T>("numeric-input-table-" + GetHashCode()), _table);
            Add(Table);

            Total = 
                new TextUiElement(uiElementFactory.GetClass(style.TotalNumber), new ButtonController(), string.Empty);
            Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.TotalContainer),
                    new ButtonController(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    new TextUiElement(uiElementFactory.GetClass(style.TotalText), new ButtonController(), "Total"),
                    Total
                });
        }

        public bool TryGetRow(T key, out NumericInputTableRow<T>? row)
        {
            return _table.TryGetRow(key, out row);
        }

        private NumericInputTableRow<T> CreateRow(T key)
        {
            return NumericInputTableRow<T>.Create(
                key, _configuration.GetName(key), _uiElementFactory, _iconFactory, _style.Row, _configuration);
        }
    }
}
