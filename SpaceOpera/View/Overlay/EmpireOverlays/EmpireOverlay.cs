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

        private static readonly string s_HoldingContainer = "empire-overlay-holding-container";
        private static readonly string s_HoldingHeader = "empire-overlay-holding-header";
        private static readonly string s_HoldingTable = "empire-overlay-holding-table";
        private static readonly ActionRow<StellarBodyHolding>.Style s_HoldingRowStyle =
            new()
            {
                Container = "empire-overlay-holding-row"
            };
        private static readonly string s_HoldingIcon = "empire-overlay-holding-row-icon";
        private static readonly string s_HoldingText = "empire-overlay-holding-row-text";

        private static readonly string s_ArmyContainer = "empire-overlay-army-container";
        private static readonly string s_ArmyHeader = "empire-overlay-army-header";
        private static readonly string s_ArmyTable = "empire-overlay-army-table";
        private static readonly ActionRow<Army>.Style s_ArmyRowStyle =
            new()
            {
                Container = "empire-overlay-army-row"
            };
        private static readonly string s_ArmyIcon = "empire-overlay-army-row-icon";
        private static readonly string s_ArmyText = "empire-overlay-army-row-text";

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
                        uiElementFactory.GetClass(s_HoldingContainer),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        new TextUiElement(
                            uiElementFactory.GetClass(s_HoldingHeader), new ButtonController(), "Holdings"),
                        new DynamicKeyedTable<StellarBodyHolding, ActionRow<StellarBodyHolding>>(
                            uiElementFactory.GetClass(s_HoldingTable), 
                            new NoOpElementController<UiSerialContainer>(),
                            UiSerialContainer.Orientation.Vertical,
                            GetHoldingRange, 
                            CreateHoldingRow, 
                            Comparer<StellarBodyHolding>.Create(
                                (x, y) => x.StellarBody.Name.CompareTo(y.StellarBody.Name)))
                    });
            Add(holdingTable);

            var armyTable =
                new DynamicUiCompoundComponent(
                    new ActionTableController(),
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_ArmyContainer),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        new TextUiElement(
                            uiElementFactory.GetClass(s_ArmyHeader), new ButtonController(), "Armies"),
                        new DynamicKeyedTable<Army, ActionRow<Army>>(
                            uiElementFactory.GetClass(s_ArmyTable),
                            new NoOpElementController<UiSerialContainer>(),
                            UiSerialContainer.Orientation.Vertical,
                            GetArmyRange,
                            CreateArmyRow,
                            Comparer<Army>.Create((x, y) => x.Name.CompareTo(y.Name)))
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
                _uiElementFactory,
                s_ArmyRowStyle,
                new List<IUiElement>()
                {
                    _iconFactory.Create(
                        _uiElementFactory.GetClass(s_ArmyIcon), new InlayController(), army),
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_ArmyText), new InlayController(), army.Name)
                },
                Enumerable.Empty<ActionRow<Army>.ActionConfiguration>());
        }

        private ActionRow<StellarBodyHolding> CreateHoldingRow(StellarBodyHolding holding)
        {
            return ActionRow<StellarBodyHolding>.Create(
                holding, 
                _uiElementFactory,
                s_HoldingRowStyle,
                new List<IUiElement>()
                {
                    _iconFactory.Create(
                        _uiElementFactory.GetClass(s_HoldingIcon), new InlayController(), holding.StellarBody),
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_HoldingText), new InlayController(), holding.StellarBody.Name)
                }, 
                Enumerable.Empty<ActionRow<StellarBodyHolding>.ActionConfiguration>());
        }
    }
}
