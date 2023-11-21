using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.View.Icons;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Universe;
using SpaceOpera.Controller.Game.Panes;
using SpaceOpera.View.Game.Panes.Common;

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
        private static readonly string s_Body = "stellar-body-pane-body";
        private static readonly ProjectsComponent.Style s_ProjectTabStyle = new()
        {
            Container = "stellar-body-pane-project-table",
            RowContainer = new()
            {
                Container = "stellar-body-pane-project-row",
                ActionContainer = "stellar-body-pane-project-row-action-container"
            },
            Icon = "stellar-body-pane-project-row-icon",
            Info = "stellar-body-pane-project-row-info",
            Text = "stellar-body-pane-project-row-text",
            Status = "stellar-body-pane-project-row-status-container",
            StatusText = "stellar-body-pane-project-row-status-text",
            StatusProgress = "stellar-body-pane-project-row-status-progress",
            Cancel = "stellar-body-pane-project-row-action-cancel"
        };

        private World? _world;
        private Faction? _faction;
        private StellarBody? _stellarBody;
        private EconomicZoneRoot? _root;
        private EconomicZoneHolding? _holding;
        private TabId _tab;

        public OverviewTab OverviewTab { get; }
        public ProjectTab ProjectTab { get; }

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
            ProjectTab = new(uiElementFactory.GetClass(s_Body), s_ProjectTabStyle, uiElementFactory, iconFactory);
        }

        public EconomicZoneHolding GetHolding()
        {
            return _holding!;
        }

        public override void Initialize()
        {
            base.Initialize();
            OverviewTab.Initialize();
            ProjectTab.Initialize();
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
            ProjectTab.SetRange(_holding == null ? null : _holding.GetProjects);
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
                case TabId.Projects:
                    SetBody(ProjectTab);
                    break;
            }
        }
    }
}
