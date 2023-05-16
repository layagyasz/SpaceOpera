using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Economics;
using Cardamom.Utils;
using SpaceOpera.Controller.Panes.LogisticsPanes;

namespace SpaceOpera.View.Panes.LogisticsPanes
{
    public class LogisticsPane : MultiTabGamePane
    {
        public enum TabId
        {
            Persistent
        }

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
        private static readonly string s_RouteIcon = "logistics-pane-roue-table-row-icon";
        private static readonly string s_RouteInfo = "logistics-pane-roue-table-row-info";

        public UiCompoundComponent Routes { get; }

        private World? _world;
        private Faction? _faction;

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

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
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            Routes = 
                new DynamicUiCompoundComponent(
                    new ActionComponentController(),
                    new DynamicUiSerialContainer(
                        _uiElementFactory.GetClass(s_RouteContainer),
                        new NoOpElementController<UiSerialContainer>(), 
                        UiSerialContainer.Orientation.Vertical)
                    {
                        ActionRow<Type>.Create(
                            typeof(PersistentRoute),
                            ActionId.Unknown, 
                            uiElementFactory,
                            s_RouteHeaderStyle,
                            new List<IUiElement>() 
                            { 
                                new SimpleUiElement(
                                    uiElementFactory.GetClass(s_RouteHeaderSpace), new InlayController())
                            },
                            s_RouteHeaderActions),
                        new DynamicUiCompoundComponent(
                            new ActionComponentController(),
                            new DynamicKeyedTable<PersistentRoute, ActionRow<PersistentRoute>>(
                                uiElementFactory.GetClass(s_RouteTable),
                                new TableController(10f),
                                UiSerialContainer.Orientation.Vertical,
                                GetRange,
                                CreateRow,
                                FluentComparator<PersistentRoute>
                                    .Comparing(x => x.LeftZone.Name)
                                    .Then(x => x.RightZone.Name)))
                    });
            var body =
                new DynamicUiContainer(uiElementFactory.GetClass(s_Body), new NoOpElementController<UiContainer>())
                {
                    Routes
                };
            SetBody(body);
        }

        public override void Populate(params object?[] args)
        {
            _world = args[0] as World;
            _faction = args[1] as Faction;
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }

        public override void SetTab(object id) { }

        private ActionRow<PersistentRoute> CreateRow(PersistentRoute route)
        {
            var left = (StellarBodyHolding)route.LeftZone;
            var right = (StellarBodyHolding)route.RightZone;
            return ActionRow<PersistentRoute>.Create(
                route,
                ActionId.Unknown,
                _uiElementFactory,
                s_RouteStyle,
                new List<IUiElement>()
                {
                    _iconFactory.Create(
                        _uiElementFactory.GetClass(s_RouteIcon), new InlayController(), left.StellarBody),
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_RouteInfo), 
                        new InlayController(),
                        $"{left.StellarBody.Name}\n{right.StellarBody.Name}"),
                    _iconFactory.Create(
                        _uiElementFactory.GetClass(s_RouteInfo), new InlayController(), right.StellarBody)
                },
                Enumerable.Empty<ActionRow<PersistentRoute>.ActionConfiguration>());
        }

        private IEnumerable<PersistentRoute> GetRange()
        {
            if (_world == null || _faction == null)
            {
                return Enumerable.Empty<PersistentRoute>();
            }
            return _world.Economy.GetPersistentRoutesFor(_faction);
        }
    }
}
