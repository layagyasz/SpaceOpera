using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game;
using SpaceOpera.Controller.Game.Panes.LogisticsPanes;
using SpaceOpera.Core;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.LogisticsPanes
{
    public class LogisticsRoutePane : SimpleGamePane
    {
        private static readonly string s_Container = "logistics-route-pane";
        private static readonly string s_Title = "logistics-route-pane-title";
        private static readonly string s_Close = "logistics-route-pane-close";
        private static readonly string s_Body = "logistics-route-pane-body";

        private static readonly string s_RouteContainer = "logistics-route-pane-route-container";

        private static readonly string s_AnchorsContainer = "logistics-route-pane-anchors-container";
        private static readonly string s_AnchorContainer = "logistics-route-pane-anchor-container";
        private static readonly string s_AnchorHeader = "logistics-route-pane-anchor-header";
        private static readonly string s_AnchorInstruction = "logistics-route-pane-anchor-instruction";
        private static readonly string s_AnchorInput = "logistics-route-pane-anchor-input";
        private static readonly string s_AnchorIcon = "logistics-route-pane-anchor-icon";
        private static readonly string s_AnchorText = "logistics-route-pane-anchor-text";

        private static readonly string s_MaterialsContainer = "logistics-route-pane-materials-container";
        private static readonly string s_MaterialContainer = "logistics-route-pane-material-container";
        private static readonly string s_MaterialHeader = "logistics-route-pane-material-header";
        private static readonly ManualNumericInputTable<IMaterial>.Style s_MaterialStyle = new()
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
                Container = "logistics-route-pane-material-row",
                Info = "logistics-route-pane-material-row-info",
                Icon = "logistics-route-pane-material-row-icon",
                Text = "logistics-route-pane-material-row-text",
                NumericInput = new()
                {
                    Container = "logistics-route-pane-material-row-numeric-input",
                    Text = "logistics-route-pane-material-row-numeric-input-text",
                    SubtractButton = "logistics-route-pane-material-row-numeric-input-subtract",
                    AddButton = "logistics-route-pane-material-row-numeric-input-add"
                },
                Remove = "logistics-route-pane-material-row-remove"
            },
            TotalContainer = "logistics-route-pane-material-total-row",
            TotalText = "logistics-route-pane-material-total-text",
            TotalNumber = "logistics-route-pane-material-total-number",
        };

        private static readonly string s_FleetHeader = "logistics-route-pane-fleet-header";
        private static readonly InterceptorMultiSelect<FleetDriver>.Style s_FleetStyle = new()
        {
            Container = "logistics-route-pane-fleet-table-container",
            Table = "logistics-route-pane-fleet-table",
            Row = new()
            {
                Container = "logistics-route-pane-fleet-row",
                ActionContainer = "logistics-route-pane-fleet-row-action-container"
            },
            Adder = "logistics-route-pane-fleet-adder"
        };
        private static readonly string s_FleetIcon = "logistics-route-pane-fleet-row-icon";
        private static readonly string s_FleetText = "logistics-route-pane-fleet-row-text";
        private static readonly List<ActionRow<FleetDriver>.ActionConfiguration> s_FleetClose = new()
        {
            new ()
            {
                Button = "logistics-route-pane-fleet-row-action-close",
                Action = ActionId.Unselect
            }
        };

        private static readonly string s_RouteSubmit = "logistics-route-pane-route-submit";

        public InterceptorInput<EconomicSubzone> LeftAnchor { get; }
        public InterceptorInput<EconomicSubzone> RightAnchor { get; }
        public ManualNumericInputTable<IMaterial> LeftMaterials { get; }
        public ManualNumericInputTable<IMaterial> RightMaterials { get; }
        public InterceptorMultiSelect<FleetDriver> Fleets { get; }
        public IUiElement Submit { get; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private World? _world;
        private Faction? _faction;
        private PersistentRoute? _route;

        public LogisticsRoutePane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new LogisticsRoutePaneController(),
                  uiElementFactory.GetClass(s_Container),
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), "Logistics Route"),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1)
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            LeftAnchor = 
                new InterceptorInput<EconomicSubzone>(
                    uiElementFactory.GetClass(s_AnchorInput), CreateAnchorInterceptor, CreateAnchorContents);
            RightAnchor =
                new InterceptorInput<EconomicSubzone>(
                    uiElementFactory.GetClass(s_AnchorInput), CreateAnchorInterceptor, CreateAnchorContents);
            var anchorsContainer =
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_AnchorsContainer),
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_AnchorContainer),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        uiElementFactory.CreateTextButton(s_AnchorHeader, "Left Anchor").Item1,
                        LeftAnchor
                    },
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_AnchorContainer),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        uiElementFactory.CreateTextButton(s_AnchorHeader, "Right Anchor").Item1,
                        RightAnchor
                    }
                };

            LeftMaterials =
                new ManualNumericInputTable<IMaterial>(
                    s_MaterialStyle,
                    x => x.Name,
                    uiElementFactory,
                    iconFactory,
                    Comparer<IMaterial>.Create((x, y) => x.Name.CompareTo(y.Name)));
            RightMaterials =
                new ManualNumericInputTable<IMaterial>(
                    s_MaterialStyle,
                    x => x.Name,
                    uiElementFactory,
                    iconFactory,
                    Comparer<IMaterial>.Create((x, y) => x.Name.CompareTo(y.Name)));
            var materialsContainer =
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_MaterialsContainer),
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_MaterialContainer), 
                        new NoOpElementController<UiSerialContainer>(), 
                        UiSerialContainer.Orientation.Vertical) 
                    {
                        uiElementFactory.CreateTextButton(s_MaterialHeader, "Left Materials").Item1,
                        LeftMaterials 
                    },
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_MaterialContainer),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        uiElementFactory.CreateTextButton(s_MaterialHeader, "Right Materials").Item1,
                        RightMaterials
                    },
                };

            Fleets = new InterceptorMultiSelect<FleetDriver>(
                s_FleetStyle,
                CreateFleetRow,
                CreateFleetInterceptor,
                uiElementFactory,
                Comparer<FleetDriver>.Create((x, y) => x.Formation.Name.CompareTo(y.Formation.Name)));

            Submit = new TextUiElement(uiElementFactory.GetClass(s_RouteSubmit), new ButtonController(), "Submit");

            var body =
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Body),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical)
                {
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_RouteContainer),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        anchorsContainer,
                        materialsContainer,
                        uiElementFactory.CreateTextButton(s_FleetHeader, "Fleets").Item1,
                        Fleets
                    },
                    Submit
                };
            SetBody(body);
        }

        public Faction GetFaction()
        {
            return _faction!;
        }

        public PersistentRoute? GetSeedRoute()
        {
            return _route;
        }

        public override void Populate(params object?[] args)
        {
            _world = (World?)args[0];
            _faction = (Faction?)args[1];
            _route = (PersistentRoute?)args[2];
            LeftMaterials.SetOptions(_world?.CoreData.Materials.Values ?? Enumerable.Empty<IMaterial>());
            RightMaterials.SetOptions(_world?.CoreData.Materials.Values ?? Enumerable.Empty<IMaterial>());
            Populated?.Invoke(this, EventArgs.Empty);
        }

        private IEnumerable<IUiElement> CreateAnchorContents(EconomicSubzone? subzone)
        {
            if (subzone == null)
            {
                yield return new TextUiElement(
                    _uiElementFactory.GetClass(s_AnchorInstruction),
                    new InlayController(),
                    "Right click to select anchor");
            }
            else
            {
                var holding = (StellarBodyRegionHolding)subzone;
                yield return _iconFactory.Create(
                    _uiElementFactory.GetClass(s_AnchorIcon), new InlayController(), holding.Region.Parent!);
                yield return new TextUiElement(
                    _uiElementFactory.GetClass(s_AnchorText), new InlayController(), holding.Region.Name);
            }
        }

        private IValueInterceptor<EconomicSubzone> CreateAnchorInterceptor()
        {
            return new BasicInterceptor<StellarBodySubRegion, EconomicSubzone>(
                x => _world?.Economy.GetHolding(_faction!, x.ParentRegion!), x => true);
        }

        private ActionRow<FleetDriver> CreateFleetRow(FleetDriver driver)
        {
            return ActionRow<FleetDriver>.Create(
                driver,
                ActionId.Select,
                _uiElementFactory,
                s_FleetStyle.Row!,
                new List<IUiElement>()
                {
                    _iconFactory.Create(_uiElementFactory.GetClass(s_FleetIcon), new InlayController(), driver),
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_FleetText), new InlayController(), driver.AtomicFormation.Name)
                },
                s_FleetClose);
        }

        private IValueInterceptor<FleetDriver> CreateFleetInterceptor()
        {
            return new BasicInterceptor<FleetDriver, FleetDriver>(x => x, x => x.Formation.Faction == _faction);
        }
    }
}
