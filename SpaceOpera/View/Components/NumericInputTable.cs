using Cardamom.Mathematics;
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
        public interface IConfiguration
        {
            IEnumerable<T> GetKeys();
            IntInterval GetRange();
            string GetName(T key);
            IntInterval GetRange(T key);
            int GetValue(T key);
            IComparer<T> GetComparer();
        }

        public struct Style
        {
            public string Container { get; set; }
            public string Header { get; set; }
            public string Table { get; set; }
            public NumericInputTableRow<T>.Style Row { get; set; }
            public string TotalContainer { get; set; }
            public string TotalText { get; set; }
            public string TotalNumber { get; set; }
            public string Submit { get; set; }
        }

        public DynamicUiCompoundComponent Table { get; }
        public TextUiElement Total { get; }
        public IUiElement Submit { get; }

        private readonly UiElementFactory _uiElementFactory;
        private IconFactory _iconFactory;
        private readonly Style _style;
        private readonly IConfiguration _configuration;

        public NumericInputTable(
            string header, 
            UiElementFactory uiElementFactory,
            ref IconFactory iconFactory, 
            Style style,
            IConfiguration configuration)
            : base(
                  new NumericInputTableController<T>(configuration),
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(style.Container),
                      new NoOpElementController<DynamicUiSerialContainer>(),
                      UiSerialContainer.Orientation.Vertical))
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
            _style = style;
            _configuration = configuration;

            Add(new TextUiElement(uiElementFactory.GetClass(style.Header), new ButtonController(), header));

            Table =
                new(
                    new RadioController<T>("numeric-input-table-" + GetHashCode()),
                    new DynamicKeyedTable<T, NumericInputTableRow<T>>(
                        uiElementFactory.GetClass(style.Table),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        configuration.GetKeys,
                        CreateRow,
                        _configuration.GetComparer()));
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

            Submit = new TextUiElement(uiElementFactory.GetClass(style.Submit), new ButtonController(), "Submit");
            Add(Submit);
        }

        private NumericInputTableRow<T> CreateRow(T key)
        {
            return NumericInputTableRow<T>.Create(
                key, _configuration.GetName(key), _uiElementFactory, _iconFactory, _style.Row, _configuration);
        }
    }
}
