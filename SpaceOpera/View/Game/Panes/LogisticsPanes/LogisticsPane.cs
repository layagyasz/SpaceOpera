using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.Core.Economics;
using Cardamom.Utils;
using SpaceOpera.Controller.Game.Panes.LogisticsPanes;
using SpaceOpera.View.Components.Dynamics;

namespace SpaceOpera.View.Game.Panes.LogisticsPanes
{
    public class LogisticsPane : MultiTabGamePane
    {
        private static readonly string s_Container = "logistics-pane";
        private static readonly string s_Title = "logistics-pane-title";
        private static readonly string s_Close = "logistics-pane-close";
        private static readonly string s_TabContainer = "logistics-pane-tab-container";
        private static readonly string s_TabOption = "logistics-pane-tab-option";
        private static readonly string s_Body = "logistics-pane-body";

        private static readonly string s_RouteContainer = "logistics-pane-route-container";
        private static readonly ActionRow<Type>.Style s_RouteHeaderStyle =
            new()
            {
                Container = "logistics-pane-route-header",
                ActionContainer = "logistics-pane-route-header-action-container"
            };
        private static readonly string s_RouteHeaderSpace = "logistics-pane-route-header-space";
        private static readonly List<ActionRow<Type>.ActionConfiguration> s_RouteHeaderActions =
            new()
            {
                new()
                {
                    Button = "logistics-pane-route-header-action-add",
                    Action = ActionId.Add
                }
            };

        private static readonly string s_RouteTable = "logistics-pane-route-table";
        private static readonly ActionRow<PersistentRoute>.Style s_RouteStyle =
            new()
            {
                Container = "logistics-pane-route-table-row"
            };
        private static readonly string s_RouteIcon = "logistics-pane-route-table-row-icon";
        private static readonly string s_RouteInfo = "logistics-pane-route-table-row-info";

        public enum TabId
        {
            Persistent
        }

        class LogisticsRouteRange
        {
            public World? World { get; set; }
            public Faction? Faction { get; set; }

            public IEnumerable<PersistentRoute> GetRange()
            {
                if (World == null || Faction == null)
                {
                    return Enumerable.Empty<PersistentRoute>();
                }
                return World.Economy.GetPersistentRoutesFor(Faction);
            }
        }

        public UiCompoundComponent Routes { get; }

        private readonly LogisticsRouteRange _range = new();

        public LogisticsPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new LogisticPaneController(),
                  uiElementFactory.GetClass(s_Container),
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), "Logistics"),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1,
                  TabBar<TabId>.Create(
                    new List<TabBar<TabId>.Definition>()
                    { 
                        new(TabId.Persistent, "Persistent")
                    },
                    uiElementFactory.GetClass(s_TabContainer),
                    uiElementFactory.GetClass(s_TabOption)))
        {
            Routes = 
                new ActionTable<PersistentRoute>(
                    uiElementFactory.GetClass(s_RouteContainer), 
                    ActionRow<Type>.Create(
                        typeof(PersistentRoute),
                        ActionId.Unknown,
                        ActionId.Unknown,
                        uiElementFactory,
                        s_RouteHeaderStyle,
                        new List<IUiElement>()
                        {
                            new SimpleUiElement(
                                uiElementFactory.GetClass(s_RouteHeaderSpace), new InlayController())
                        },
                        s_RouteHeaderActions),
                    DynamicKeyedContainer<PersistentRoute>.CreateSerial(
                        uiElementFactory.GetClass(s_RouteTable),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        _range.GetRange,
                        new SimpleKeyedElementFactory<PersistentRoute>(uiElementFactory, iconFactory, CreateRow),
                        FluentComparator<PersistentRoute>
                            .Comparing(x => x.LeftAnchor.Parent.Name)
                            .Then(x => x.RightAnchor.Parent.Name)));
            var body =
                new DynamicUiContainer(uiElementFactory.GetClass(s_Body), new NoOpElementController())
                {
                    Routes
                };
            SetBody(body);
        }

        public override void Populate(params object?[] args)
        {
            _range.World = args[0] as World;
            _range.Faction = args[1] as Faction;
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }

        public override object GetTab()
        {
            return TabId.Persistent;
        }

        public override void SetTab(object id) { }

        private static IKeyedUiElement<PersistentRoute> CreateRow(
            PersistentRoute route, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            var left = ((StellarBodyRegionHolding)route.LeftAnchor).Region;
            var right = ((StellarBodyRegionHolding)route.RightAnchor).Region;
            return ActionRow<PersistentRoute>.Create(
                route,
                ActionId.Unknown,
                ActionId.Unknown,
                uiElementFactory,
                s_RouteStyle,
                new List<IUiElement>()
                {
                        iconFactory.Create(
                            uiElementFactory.GetClass(s_RouteIcon), new InlayController(), left.Parent!),
                        new TextUiElement(
                            uiElementFactory.GetClass(s_RouteInfo),
                            new InlayController(),
                            $"{left.Parent!.Name}\n{right.Parent!.Name}"),
                        iconFactory.Create(
                            uiElementFactory.GetClass(s_RouteInfo), new InlayController(), right.Parent!)
                },
                Enumerable.Empty<ActionRow<PersistentRoute>.ActionConfiguration>());
        }
    }
}
