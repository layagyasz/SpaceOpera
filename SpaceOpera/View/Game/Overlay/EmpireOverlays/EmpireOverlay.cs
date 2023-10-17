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

namespace SpaceOpera.View.Game.Overlay.EmpireOverlays
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
                  new ActionComponentController(),
                  new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Container),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical))
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            var holdingTable = 
                new DynamicUiCompoundComponent(
                    new ActionComponentController(),
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_TableContainer),
                        new NoOpElementController(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        new TextUiElement(
                            uiElementFactory.GetClass(s_TableHeader), new ButtonController(), "Holdings"),
                        new DynamicUiCompoundComponent(
                            new ActionComponentController(),
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
                    new ActionComponentController(),
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_TableContainer),
                        new NoOpElementController(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        new TextUiElement(
                            uiElementFactory.GetClass(s_TableHeader), new ButtonController(), "Fleets"),
                        new DynamicUiCompoundComponent(
                            new ActionComponentController(),
                            new DynamicKeyedTable<AtomicFormationDriver, ActionRow<AtomicFormationDriver>>(
                                uiElementFactory.GetClass(s_Table),
                                new TableController(10f),
                                UiSerialContainer.Orientation.Vertical,
                                GetFleetRange,
                                CreateFleetRow,
                                Comparer<AtomicFormationDriver>.Create(
                                    (x, y) => x.AtomicFormation.Name.CompareTo(y.AtomicFormation.Name))))
            });
            Add(fleetTable);

            var armyTable =
                new DynamicUiCompoundComponent(
                    new ActionComponentController(),
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_TableContainer),
                        new NoOpElementController(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        new TextUiElement(
                            uiElementFactory.GetClass(s_TableHeader), new ButtonController(), "Armies"),
                        new DynamicUiCompoundComponent(
                            new ActionComponentController(),
                            new DynamicKeyedTable<ArmyDriver, ActionRow<ArmyDriver>>(
                                uiElementFactory.GetClass(s_Table),
                                new TableController(10f),
                                UiSerialContainer.Orientation.Vertical,
                                GetArmyRange,
                                CreateArmyRow,
                                Comparer<ArmyDriver>.Create((x, y) => x.Army.Name.CompareTo(y.Army.Name))))
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

        private IEnumerable<ArmyDriver> GetArmyRange()
        {
            if (_world == null || _faction == null)
            {
                return Enumerable.Empty<ArmyDriver>();
            }
            return _world.FormationManager.GetArmyDriversFor(_faction);
        }

        private IEnumerable<AtomicFormationDriver> GetFleetRange()
        {
            if (_world == null || _faction == null)
            {
                return Enumerable.Empty<AtomicFormationDriver>();
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

        private ActionRow<ArmyDriver> CreateArmyRow(ArmyDriver driver)
        {
            return ActionRow<ArmyDriver>.Create(
                driver,
                ActionId.Select,
                _uiElementFactory,
                new() { Container = s_Row },
                new List<IUiElement>()
                {
                    _iconFactory.Create(
                        _uiElementFactory.GetClass(s_Icon), new InlayController(), driver.Army),
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_Text), new InlayController(), driver.Army.Name)
                },
                Enumerable.Empty<ActionRow<ArmyDriver>.ActionConfiguration>());
        }

        private ActionRow<AtomicFormationDriver> CreateFleetRow(AtomicFormationDriver driver)
        {
            return ActionRow<AtomicFormationDriver>.Create(
                driver,
                ActionId.Select,
                _uiElementFactory,
                new() { Container = s_Row },
                new List<IUiElement>()
                {
                    _iconFactory.Create(
                        _uiElementFactory.GetClass(s_Icon), new InlayController(), driver.AtomicFormation),
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_Text), new InlayController(), driver.AtomicFormation.Name)
                },
                Enumerable.Empty<ActionRow<AtomicFormationDriver>.ActionConfiguration>());
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
