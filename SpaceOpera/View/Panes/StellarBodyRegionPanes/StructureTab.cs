using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Core;
using SpaceOpera.Core.Economics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.StellarBodyRegionPanes
{
    public class StructureTab : DynamicUiContainer
    {
        private static readonly string s_ClassName = "stellar-body-region-pane-body";
        private static readonly string s_StructureTableClassName = "stellar-body-region-pane-structure-table";

        private static readonly NumericInputTableRow<Structure>.Style s_StructureTableStyle =
            new()
            {
                Container = "stellar-body-region-pane-structure-table-row",
                Info = "stellar-body-region-pane-structure-table-row-info",
                Icon = "stellar-body-region-pane-structure-table-row-icon",
                Text = "stellar-body-region-pane-structure-table-row-text",
                NumericInput = new()
                {
                    Container = "stellar-body-region-pane-structure-table-row-numeric-input",
                    Text = "stellar-body-region-pane-structure-table-row-numeric-input-text",
                    SubtractButton = "stellar-body-region-pane-structure-table-row-numeric-input-subtract",
                    AddButton = "stellar-body-region-pane-structure-table-row-numeric-input-add"
                }
            };

        private IconFactory _iconFactory;

        private World? _world;
        private StellarBodyRegionHolding? _holding;

        public StructureTab(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(uiElementFactory.GetClass(s_ClassName), new NoOpElementController<StructureTab>())
        {
            _iconFactory = iconFactory;
            var structureTable =
                new DynamicUiSerialContainer<Structure, NumericInputTableRow<Structure>>(
                    uiElementFactory.GetClass(s_StructureTableClassName),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical,
                    GetRange,
                    x => NumericInputTableRow<Structure>.Create(
                        x, x.Name, uiElementFactory, ref _iconFactory, s_StructureTableStyle),
                    Comparer<Structure>.Create((x, y) => x.Name.CompareTo(y.Name)));
            Add(structureTable);
        }

        public void Populate(World? world, StellarBodyRegionHolding? holding)
        {
            _world = world;
            _holding = holding;
            if (_holding != null)
            {
                _iconFactory = _iconFactory.ForFaction(_holding.Parent.Owner);
            }
        }

        public IEnumerable<Structure> GetRange()
        {
            return _world?.GetStructures() ?? Enumerable.Empty<Structure>();
        }
    }
}
