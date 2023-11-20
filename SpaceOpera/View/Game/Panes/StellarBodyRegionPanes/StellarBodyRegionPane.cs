using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.View.Icons;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Universe;
using SpaceOpera.Controller.Game.Panes.StellarBodyRegionPanes;
using SpaceOpera.View.Game.Panes.Common;

namespace SpaceOpera.View.Game.Panes.StellarBodyRegionPanes
{
    public class StellarBodyRegionPane : MultiTabGamePane
    {
        enum TabId
        {
            Structures,
            Projects
        }

        private static readonly string s_Container = "stellar-body-region-pane";
        private static readonly string s_Title = "stellar-body-region-pane-title";
        private static readonly string s_Close = "stellar-body-region-pane-close";
        private static readonly string s_TabContainer = "stellar-body-region-pane-tab-container";
        private static readonly string s_TabOption = "stellar-body-region-pane-tab-option";
        private static readonly string s_Body = "stellar-body-region-pane-body";

        private static readonly ProjectsComponent.Style s_ProjectTabStyle = new()
        {
            Container = "stellar-body-region-pane-project-table",
            RowContainer = new()
            {
                Container = "stellar-body-region-pane-project-row",
                ActionContainer = "stellar-body-region-pane-project-row-action-container"
            },
            Icon = "stellar-body-region-pane-project-row-icon",
            Info = "stellar-body-region-pane-project-row-info",
            Text = "stellar-body-region-pane-project-row-text",
            Status = "stellar-body-region-pane-project-row-status-container",
            StatusText = "stellar-body-region-pane-project-row-status-text",
            StatusProgress = "stellar-body-region-pane-project-row-status-progress",
            Cancel = "stellar-body-region-pane-project-row-action-cancel"
        };

        private World? _world;
        private Faction? _faction;
        private StellarBodyRegion? _region;
        private EconomicSubzoneHolding? _holding;
        private TabId _tab;

        public StructureTab StructureTab { get; }
        public ProjectTab ProjectTab { get; }

        public StellarBodyRegionPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new StellarBodyRegionPaneController(),
                  uiElementFactory.GetClass(s_Container),
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), string.Empty),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1,
                  TabBar<TabId>.Create(
                    new List<TabBar<TabId>.Definition>()
                    {
                        new(TabId.Structures, "Structures"),
                        new(TabId.Projects, "Projects")
                    },
                    uiElementFactory.GetClass(s_TabContainer),
                    uiElementFactory.GetClass(s_TabOption)))
        {
            StructureTab = new(uiElementFactory, iconFactory);
            ProjectTab = new(uiElementFactory.GetClass(s_Body), s_ProjectTabStyle, uiElementFactory, iconFactory);
        }

        public override void Initialize()
        {
            base.Initialize();
            StructureTab.Initialize();
            ProjectTab.Initialize();
        }

        public EconomicSubzoneHolding GetHolding()
        {
            return _holding!;
        }

        public override void Populate(params object?[] args)
        {
            _world = args[0] as World;
            _faction = args[1] as Faction;
            _region = args[2] as StellarBodyRegion;
            if (_world != null && _faction != null && _region != null)
            {
                _holding = _world.Economy.GetRoot(_region.Parent!)?.GetChild(_region)?.GetHolding(_faction);
            }
            StructureTab.Populate(_world, _holding);
            ProjectTab.SetRange(_holding == null ? null : _holding.GetProjects);

            SetTitle(_region?.Name ?? "Unknown Region");
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
                case TabId.Structures:
                    SetBody(StructureTab);
                    break;
                case TabId.Projects:
                    SetBody(ProjectTab);
                    break;
            }
        }
    }
}
