using Cardamom.Mathematics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Components
{
    public class NumericInputTable<T> : DynamicUiCompoundComponent where T : notnull
    {
        public interface IConfiguration
        {
            IEnumerable<T> GetRange();
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
        }

        public DynamicKeyedTable<T, NumericInputTableRow<T>> Table { get; }

        private readonly UiElementFactory _uiElementFactory;
        private IconFactory _iconFactory;
        private readonly Style _style;
        private readonly IConfiguration _configuration;

        public NumericInputTable(
            UiElementFactory uiElementFactory, ref IconFactory iconFactory, Style style, IConfiguration configuration)
            : base(
                  new NumericInputTableController<T>(),
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(style.Container), 
                      new NoOpElementController<DynamicUiSerialContainer>(), 
                      UiSerialContainer.Orientation.Vertical))
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
            _style = style;
            _configuration = configuration;
            Table = 
                new DynamicKeyedTable<T, NumericInputTableRow<T>>(
                    uiElementFactory.GetClass(style.Table),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical,
                    configuration.GetRange,
                    CreateRow,
                    _configuration.GetComparer());
            Add(Table);
        }

        private NumericInputTableRow<T> CreateRow(T key)
        {
            return NumericInputTableRow<T>.Create(
                key, _configuration.GetName(key), _uiElementFactory, ref _iconFactory, _style.Row, _configuration);
        }
    }
}
