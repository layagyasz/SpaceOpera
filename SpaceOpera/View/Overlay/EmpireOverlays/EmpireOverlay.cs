using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using OpenTK.Mathematics;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Overlay.EmpireOverlays
{
    public class EmpireOverlay : DynamicUiCompoundComponent, IOverlay
    {
        private static readonly string s_Container = "empire-overlay-container";

        private static readonly string s_TableContainer = "empire-overlay-table-container";
        private static readonly string s_TableHeader = "empire-overlay-table-header";
        private static readonly string s_Table = "empire-overlay-table";
        private static readonly string s_Row = "empire-overlay-row";
        private static readonly string s_Icon = "empire-overlay-row-icon";
        private static readonly string s_Text = "empire-overlay-row-text";

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private World? _world;
        private Faction? _faction;
        private Vector2 _bounds;

        public EmpireOverlay(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new ActionTableController(),
                  new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Container),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical))
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            var holdingTable = 
                new DynamicUiCompoundComponent(
                    new ActionTableController(),
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_TableContainer),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        new TextUiElement(
                            uiElementFactory.GetClass(s_TableHeader), new ButtonController(), "Holdings"),
                        new DynamicUiCompoundComponent(
                            new ActionTableController(),
                            new DynamicKeyedTable<StellarBodyHolding, ActionRow<StellarBodyHolding>>(
                                uiElementFactory.GetClass(s_Table), 
                                new TableController(10f),
                                UiSerialContainer.Orientation.Vertical,
                                GetHoldingRange, 
                                CreateHoldingRow, 
                                Comparer<StellarBodyHolding>.Create(
                                    (x, y) => x.StellarBody.Name.CompareTo(y.StellarBody.Name))))
                    });
            Add(holdingTable);

            var fleetTable =
                new DynamicUiCompoundComponent(
                    new ActionTableController(),
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_TableContainer),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        new TextUiElement(
                            uiElementFactory.GetClass(s_TableHeader), new ButtonController(), "Fleets"),
                        new DynamicUiCompoundComponent(
                            new ActionTableController(),
                            new DynamicKeyedTable<FormationDriver, ActionRow<FormationDriver>>(
                                uiElementFactory.GetClass(s_Table),
                                new TableController(10f),
                                UiSerialContainer.Orientation.Vertical,
                                GetFleetRange,
                                CreateFleetRow,
                                Comparer<FormationDriver>.Create(
                                    (x, y) => x.Formation.Name.CompareTo(y.Formation.Name))))
            });
            Add(fleetTable);

            var armyTable =
                new DynamicUiCompoundComponent(
                    new ActionTableController(),
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_TableContainer),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        new TextUiElement(
                            uiElementFactory.GetClass(s_TableHeader), new ButtonController(), "Armies"),
                        new DynamicUiCompoundComponent(
                            new ActionTableController(),
                            new DynamicKeyedTable<Army, ActionRow<Army>>(
                                uiElementFactory.GetClass(s_Table),
                                new TableController(10f),
                                UiSerialContainer.Orientation.Vertical,
                                GetArmyRange,
                                CreateArmyRow,
                                Comparer<Army>.Create((x, y) => x.Name.CompareTo(y.Name))))
                    });
            Add(armyTable);
        }

        public override void Draw(IRenderTarget target, IUiContext context)
        {
            Position = new(_bounds.X - Size.X, 0.5f * (_bounds.Y - Size.Y), 0);
            base.Draw(target, context);
        }

        public void Populate(params object?[] args)
        {
            _world = (World?)args[0];
            _faction = (Faction?)args[1];
            Refresh();
        }

        public override void ResizeContext(Vector3 bounds)
        {
            _bounds = bounds.Xy;
        }

        private IEnumerable<Army> GetArmyRange()
        {
            if (_world == null || _faction == null)
            {
                return Enumerable.Empty<Army>();
            }
            return _world.FormationManager.GetArmiesFor(_faction);
        }

        private IEnumerable<FormationDriver> GetFleetRange()
        {
            if (_world == null || _faction == null)
            {
                return Enumerable.Empty<FormationDriver>();
            }
            return _world.FormationManager.GetFleetDriversFor(_faction);
        }

        private IEnumerable<StellarBodyHolding> GetHoldingRange()
        {
            if (_world == null || _faction == null)
            {
                return Enumerable.Empty<StellarBodyHolding>();
            }
            return _world.Economy.GetHoldingsFor(_faction);
        }

        private ActionRow<Army> CreateArmyRow(Army army)
        {
            return ActionRow<Army>.Create(
                army,
                ActionId.Select,
                _uiElementFactory,
                new() { Container = s_Row },
                new List<IUiElement>()
                {
                    _iconFactory.Create(
                        _uiElementFactory.GetClass(s_Icon), new InlayController(), army),
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_Text), new InlayController(), army.Name)
                },
                Enumerable.Empty<ActionRow<Army>.ActionConfiguration>());
        }

        private ActionRow<FormationDriver> CreateFleetRow(FormationDriver driver)
        {
            return ActionRow<FormationDriver>.Create(
                driver,
                ActionId.Select,
                _uiElementFactory,
                new() { Container = s_Row },
                new List<IUiElement>()
                {
                    _iconFactory.Create(
                        _uiElementFactory.GetClass(s_Icon), new InlayController(), driver.Formation),
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_Text), new InlayController(), driver.Formation.Name)
                },
                Enumerable.Empty<ActionRow<FormationDriver>.ActionConfiguration>());
        }

        private ActionRow<StellarBodyHolding> CreateHoldingRow(StellarBodyHolding holding)
        {
            return ActionRow<StellarBodyHolding>.Create(
                holding,
                ActionId.Select,
                _uiElementFactory,
                new() { Container = s_Row },
                new List<IUiElement>()
                {
                    _iconFactory.Create(
                        _uiElementFactory.GetClass(s_Icon), new InlayController(), holding.StellarBody),
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_Text), new InlayController(), holding.StellarBody.Name)
                }, 
                Enumerable.Empty<ActionRow<StellarBodyHolding>.ActionConfiguration>());
        }
    }
}
