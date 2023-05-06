using Cardamom.Mathematics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.Core;
using SpaceOpera.Core.Economics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.StellarBodyRegionPanes
{
    public class StructureTab : DynamicUiSerialContainer
    {
        private static readonly string s_Container = "stellar-body-region-pane-body";

        private static readonly NumericInputTable<Structure>.Style s_StructureTableStyle =
            new()
            {
                Container = "stellar-body-region-pane-structure-container",
                Header = "stellar-body-region-pane-structure-table-header",
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
                },
                TotalContainer = "stellar-body-region-pane-structure-table-total-row",
                TotalText = "stellar-body-region-pane-structure-table-total-text",
                TotalNumber = "stellar-body-region-pane-structure-table-total-number",
                Submit = "stellar-body-region-pane-structure-table-submit"
            };

        private static readonly NumericInputTable<Recipe>.Style s_RecipeTableStyle =
            new()
            {
                Container = "stellar-body-region-pane-structure-container",
                Header = "stellar-body-region-pane-structure-table-header",
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
                },
                TotalContainer = "stellar-body-region-pane-structure-table-total-row",
                TotalText = "stellar-body-region-pane-structure-table-total-text",
                TotalNumber = "stellar-body-region-pane-structure-table-total-number",
                Submit = "stellar-body-region-pane-structure-table-submit"
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

            public IEnumerable<Structure> GetKeys()
            {
                return _world?.GetStructures() ?? Enumerable.Empty<Structure>();
            }

            public IntInterval GetRange()
            {
                return _holding == null ? new(0, 0) : new(0, _holding.GetStructureNodes());
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

        class RecipeTableConfiguration : NumericInputTable<Recipe>.IConfiguration
        {
            private World? _world;
            private StellarBodyRegionHolding? _holding;
            private Structure? _structure;

            public void Populate(World? world, StellarBodyRegionHolding? holding)
            {
                _world = world;
                _holding = holding;
            }

            public void SetStructure(Structure? structure)
            {
                _structure = structure;
            }


            public IEnumerable<Recipe> GetKeys()
            {
                if (_world == null || _holding == null || _structure == null)
                {
                    return Enumerable.Empty<Recipe>();
                }
                return _world
                    .GetRecipesFor(_holding?.Parent.Owner)
                    .Where(x => x.Structure == _structure)
                    .Where(x => _holding?.GetResourceNodes(x.BoundResourceNode) > 0);
            }

            public IntInterval GetRange()
            {
                return _holding == null || _structure == null
                    ? new(0, 0) : new(0, _holding.GetStructureCount(_structure));
            }

            public string GetName(Recipe key)
            {
                return key.Name;
            }

            public IntInterval GetRange(Recipe key)
            {
                if (_holding == null || _structure == null)
                {
                    return new(0, int.MaxValue);
                }
                return new(0, _holding.GetResourceNodes(key.BoundResourceNode));
            }

            public int GetValue(Recipe key)
            {
                return _holding == null ? 0 : _holding.GetProduction(key);
            }

            public IComparer<Recipe> GetComparer()
            {
                return Comparer<Recipe>.Create((x, y) => x.Name.CompareTo(y.Name));
            }
        }

        private readonly IconFactory _iconFactory;
        private readonly StructureTableConfiguration _structureTableConfiguration;
        private readonly RecipeTableConfiguration _recipeTableConfiguration;

        public NumericInputTable<Structure> StructureTable { get; }
        public NumericInputTable<Recipe> RecipeTable { get; }

        public StructureTab(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  uiElementFactory.GetClass(s_Container),
                  new NoOpElementController<StructureTab>(),
                  Orientation.Horizontal)
        {
            _iconFactory = iconFactory;

            _structureTableConfiguration = new();
            StructureTable =
                new(
                    "Structures",
                    uiElementFactory,
                    ref _iconFactory,
                    s_StructureTableStyle,
                    _structureTableConfiguration);
            Add(StructureTable);

            _recipeTableConfiguration = new();
            RecipeTable =
                new(
                    "Production",
                    uiElementFactory,
                    ref _iconFactory,
                    s_RecipeTableStyle,
                    _recipeTableConfiguration);
            Add(RecipeTable);
        }

        public void Populate(World? world, StellarBodyRegionHolding? holding)
        {
            _structureTableConfiguration.Populate(world, holding);
            _recipeTableConfiguration.Populate(world, holding);
        }

        public void SetStructure(Structure? structure)
        {
            _recipeTableConfiguration.SetStructure(structure);
        }
    }
}
