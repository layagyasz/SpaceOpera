using Cardamom.Mathematics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Panes.LogisticsPanes;
using SpaceOpera.Core.Economics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.LogisticsPanes
{
    public class MaterialComponent : DynamicUiCompoundComponent
    {
        private static readonly string s_Container = "logistics-route-pane-material-container";
        private static readonly string s_Header = "logistics-route-pane-material-header";

        private static readonly string s_SelectWrapper =
            "logistics-route-pane-material-select-wrapper";
        private static readonly string s_Select = "logistics-route-pane-material-select";
        private static readonly string s_SelectDropBox =
            "logistics-route-pane-material-select-dropbox";
        private static readonly string s_SelectDropOption =
            "logistics-route-pane-material-select-option";
        private static readonly string s_Add = "logistics-route-pane-material-add";

        private static readonly NumericInputTable<IMaterial>.Style s_TableStyle =
            new()
            {
                Container = "logistics-route-pane-material-table-container",
                Table = "logistics-route-pane-material-table",
                Row = new()
                {
                    Container = "logistics-route-pane-material-table-row",
                    Info = "logistics-route-pane-material-table-row-info",
                    Icon = "logistics-route-pane-material-table-row-icon",
                    Text = "logistics-route-pane-material-table-row-text",
                    NumericInput = new()
                    {
                        Container = "logistics-route-pane-material-table-row-numeric-input",
                        Text = "logistics-route-pane-material-table-row-numeric-input-text",
                        SubtractButton = "logistics-route-pane-material-table-row-numeric-input-subtract",
                        AddButton = "logistics-route-pane-material-table-row-numeric-input-add"
                    }
                },
                TotalContainer = "logistics-route-pane-material-table-total-row",
                TotalText = "logistics-route-pane-material-table-total-text",
                TotalNumber = "logistics-route-pane-material-table-total-number",
            };

        class MaterialTableConfiguration : NumericInputTable<IMaterial>.IRowConfiguration
        {
            public string GetName(IMaterial key)
            {
                return key.Name;
            }

            public IntInterval GetRange(IMaterial key)
            {
                return IntInterval.Unbounded;
            }

            public int GetValue(IMaterial key)
            {
                return 0;
            }

            public IComparer<IMaterial> GetComparer()
            {
                return Comparer<IMaterial>.Create((x, y) => x.Name.CompareTo(y.Name));
            }
        }

        public Select Select { get; }
        public IUiElement AddButton { get; }
        public NumericInputTable<IMaterial> Table { get; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly MaterialTableConfiguration _configuration = new();
        private readonly HashSet<IMaterial> _range = new();

        public MaterialComponent(string title, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new MaterialComponentController("material-component"), 
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(s_Container), 
                      new NoOpElementController<UiSerialContainer>(),
                      UiSerialContainer.Orientation.Vertical))
        {
            _uiElementFactory = uiElementFactory;

            Add(uiElementFactory.CreateTextButton(s_Header, title).Item1);

            Select =
                (Select)uiElementFactory.CreateSelect<IMaterial>(
                    s_Select, s_SelectDropBox, Enumerable.Empty<IUiElement>()).Item1;
            AddButton = uiElementFactory.CreateTextButton(s_Add, "+").Item1;
            Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_SelectWrapper), 
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Horizontal) 
                { 
                    Select, 
                    AddButton 
                });

            Table = 
                new NumericInputTable<IMaterial>(
                    GetKeys, 
                    () => IntInterval.Unbounded, 
                    uiElementFactory,
                    iconFactory,
                    s_TableStyle, 
                    _configuration);
            Add(Table);
        }

        public void Add(IMaterial material)
        {
            _range.Add(material);
            Table.Refresh();
        }

        public void Populate(IEnumerable<IMaterial> materials)
        {
            Select.Clear(/* dispose= */ true);
            foreach (var material in materials)
            {
                var option = _uiElementFactory.CreateSelectOption(s_SelectDropOption, material, material.Name).Item1;
                option.Initialize();
                Select.Add(option);
            }
        }

        public void Remove(IMaterial material)
        {
            _range.Remove(material);
            Table.Refresh();
        }

        public void SetRange(IEnumerable<IMaterial> range)
        {
            _range.Clear();
            foreach (var item in range)
            {
                _range.Add(item);
            }
            Table.Refresh();
        }

        private IEnumerable<IMaterial> GetKeys()
        {
            return _range;
        }
    }
}
