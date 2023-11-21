using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes;
using SpaceOpera.Core;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.MilitaryPanes
{
    public class MilitaryPane : MultiTabGamePane
    {
        private static readonly string s_Container = "military-pane";
        private static readonly string s_Title = "military-pane-title";
        private static readonly string s_Close = "military-pane-close";
        private static readonly string s_TabContainer = "military-pane-tab-container";
        private static readonly string s_TabOption = "military-pane-tab-option";

        public enum TabId
        {
            Army,
            Fleet,
            Recruitment
        }

        class FormationRange
        {
            public World? World { get; set; }
            public Faction? Faction { get; set; }

            public IEnumerable<IFormationDriver> GetRange()
            {
                if (World == null || Faction == null)
                {
                    return Enumerable.Empty<IFormationDriver>();
                }
                return World.Formations.GetDrivers(Faction);
            }
        }

        public FormationsTab Formations { get; }
        public RecruitmentTab Recruitment { get; }

        private readonly FormationRange _range = new();

        private TabId _tab;

        public MilitaryPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new MultiTabGamePaneController(),
                  uiElementFactory.GetClass(s_Container), 
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), "Military"),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1,
                  TabBar<TabId>.Create(
                    new List<TabBar<TabId>.Definition>()
                    {
                        new(TabId.Army, "Army"),
                        new(TabId.Fleet, "Fleet"),
                        new(TabId.Recruitment, "Recruitment")
                    },
                    uiElementFactory.GetClass(s_TabContainer),
                    uiElementFactory.GetClass(s_TabOption)))
        {
            Formations = FormationsTab.Create(uiElementFactory, iconFactory);
            Recruitment = RecruitmentTab.Create(uiElementFactory, iconFactory);
        }

        public override void Populate(params object?[] args)
        {
            _range.World = args[0] as World;
            _range.Faction = args[1] as Faction;
            Recruitment.Populate(_range.World, _range.Faction);
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }

        public override object GetTab()
        {
            return _tab;
        }

        public override void Initialize()
        {
            base.Initialize();
            Formations.Initialize();
            Recruitment.Initialize();
        }

        public override void SetTab(object id)
        {
            _tab = (TabId)id;
            switch (_tab)
            {
                case TabId.Army:
                    SetBody(Formations);
                    Formations.SetRange(() => _range.GetRange().Where(x => x is ArmyDriver));
                    break;
                case TabId.Fleet:
                    SetBody(Formations);
                    Formations.SetRange(() => _range.GetRange().Where(x => x is FleetDriver));
                    break;
                case TabId.Recruitment:
                    SetBody(Recruitment);
                    break;
            }
        }
    }
}
