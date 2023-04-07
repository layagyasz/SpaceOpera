using Cardamom.Mathematics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.Core;
using SpaceOpera.Core.Economics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.StellarBodyRegionPanes
{
    public class StructureTab : DynamicUiContainer
    {
        private static readonly string s_ClassName = "stellar-body-region-pane-body";

        private static readonly NumericInputTable<Structure>.Style s_StructureTableStyle =
            new()
            {
                Container = "stellar-body-region-pane-structure-container",
                Table = "stellar-body-region-pane-structure-table",
                Row = new()
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
                }
            };

        class StructureTableConfiguration : NumericInputTable<Structure>.IConfiguration
        {
            private World? _world;
            private StellarBodyRegionHolding? _holding;

            public void Populate(World? world, StellarBodyRegionHolding? holding)
            {
                _world = world;
                _holding = holding;
            }

            public IEnumerable<Structure> GetRange()
            {
                return _world?.GetStructures() ?? Enumerable.Empty<Structure>();
            }

            public string GetName(Structure key)
            {
                return key.Name;
            }

            public IntInterval GetRange(Structure key)
            {
                return _holding == null ? new(0, 0) : new(_holding.GetStructureCount(key), int.MaxValue);
            }

            public int GetValue(Structure key)
            {
                return _holding == null ? 0 : _holding.GetStructureCount(key);
            }

            public IComparer<Structure> GetComparer()
            {
                return Comparer<Structure>.Create((x, y) => x.Name.CompareTo(y.Name));
            }
        }

        private IconFactory _iconFactory;
        private StructureTableConfiguration _structureTableConfiguration;

        public StructureTab(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(uiElementFactory.GetClass(s_ClassName), new NoOpElementController<StructureTab>())
        {
            _iconFactory = iconFactory;
            _structureTableConfiguration = new();
            var structureTable =
                new NumericInputTable<Structure>(
                    uiElementFactory, ref _iconFactory, s_StructureTableStyle, _structureTableConfiguration);
            Add(structureTable);
        }

        public void Populate(World? world, StellarBodyRegionHolding? holding)
        {
            _structureTableConfiguration.Populate(world, holding);
            if (holding != null)
            {
                _iconFactory = _iconFactory.ForFaction(holding.Parent.Owner);
            }
        }
    }
}
