using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.View.Icons;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Universe;
using SpaceOpera.Controller.Game.Panes;
using SpaceOpera.View.Game.Panes.StellarBodyRegionPanes;

namespace SpaceOpera.View.Game.Panes.StellarBodyPanes
{
    public class StellarBodyPane : MultiTabGamePane
    {
        enum TabId
        {
            Overview,
            Military,
            Projects,
            Regions,
            Inventory
        }

        private static readonly string s_Container = "stellar-body-pane";
        private static readonly string s_Title = "stellar-body-pane-title";
        private static readonly string s_Close = "stellar-body-pane-close";
        private static readonly string s_TabContainer = "stellar-body-pane-tab-container";
        private static readonly string s_TabOption = "stellar-body-pane-tab-option";

        private World? _world;
        private Faction? _faction;
        private StellarBody? _stellarBody;
        private EconomicZoneRoot? _root;
        private EconomicZoneHolding? _holding;
        private TabId _tab;

        public OverviewTab OverviewTab { get; }

        public StellarBodyPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new MultiTabGamePaneController(),
                  uiElementFactory.GetClass(s_Container),
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), string.Empty),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1,
                  TabBar<TabId>.Create(
                    new List<TabBar<TabId>.Definition>()
                    {
                        new(TabId.Overview, "Overview"),
                        new(TabId.Military, "Military"),
                        new(TabId.Projects, "Projects"),
                        new(TabId.Regions, "Regions"),
                        new(TabId.Inventory, "Inventory")
                    },
                    uiElementFactory.GetClass(s_TabContainer),
                    uiElementFactory.GetClass(s_TabOption)))
        {
            OverviewTab = new(uiElementFactory, iconFactory);
        }

        public EconomicZoneHolding GetHolding()
        {
            return _holding!;
        }

        public override void Initialize()
        {
            base.Initialize();
            OverviewTab.Initialize();
        }

        public override void Populate(params object?[] args)
        {
            _world = args[0] as World;
            _faction = args[1] as Faction;
            _stellarBody = args[2] as StellarBody;
            if (_world != null && _faction != null && _stellarBody != null)
            {
                _root = _world.Economy.GetRoot(_stellarBody);
                _holding = _root!.GetHolding(_faction)!;
            }

            OverviewTab.SetHolding(_root!, _holding!);
            SetTitle(_stellarBody?.Name ?? "Unknown Stellar Body");
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }

        public override object GetTab()
        {
            return _tab;
        }

        public override void SetTab(object id)
        {
            _tab = (TabId)id;
            switch (_tab)
            {
                case TabId.Overview:
                    SetBody(OverviewTab);
                    break;
            }
            Refresh();
        }
    }
}
