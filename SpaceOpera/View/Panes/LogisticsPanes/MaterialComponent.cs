using Cardamom.Ui;
using SpaceOpera.Core.Economics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.LogisticsPanes
{
    public class MaterialComponent : ManualNumericInputTable<IMaterial>
    {
        private static readonly Style s_TableStyle =
            new()
            {
                Container = "logistics-route-pane-material-table-container",
                Table = "logistics-route-pane-material-table",
                SelectWrapper = "logistics-route-pane-material-select-wrapper",
                Select = "logistics-route-pane-material-select",
                SelectDropBox = "logistics-route-pane-material-select-dropbox",
                SelectOption = "logistics-route-pane-material-select-option",
                Add = "logistics-route-pane-material-add",
                Row = new ManualNumericInputTableRow<IMaterial>.Style()
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
                    },
                    Remove = "logistics-route-pane-material-table-row-remove"
                },
                TotalContainer = "logistics-route-pane-material-table-total-row",
                TotalText = "logistics-route-pane-material-table-total-text",
                TotalNumber = "logistics-route-pane-material-table-total-number",
            };

        public MaterialComponent(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  s_TableStyle,
                  x => x.Name,
                  uiElementFactory,
                  iconFactory,
                  Comparer<IMaterial>.Create((x, y) => x.Name.CompareTo(y.Name)))
        { }
    }
}
