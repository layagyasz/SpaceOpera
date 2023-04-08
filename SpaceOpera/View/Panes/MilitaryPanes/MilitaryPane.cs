using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Panes;
using SpaceOpera.Core;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;
using SpaceOpera.View.Panes.DesignPanes;

namespace SpaceOpera.View.Panes.MilitaryPanes
{
    public class MilitaryPane : MultiTabGamePane
    {
        public enum TabId
        {
            Army,
            Fleet
        }

        private static readonly string s_Title = "Military";

        private static readonly string s_ClassName = "military-pane";
        private static readonly string s_TitleClassName = "military-pane-title";
        private static readonly string s_CloseClass = "military-pane-close";
        private static readonly string s_TabContainerClassName = "military-pane-tab-container";
        private static readonly string s_TabOptionClassName = "military-pane-tab-option";
        private static readonly string s_BodyClassName = "military-pane-body";
        private static readonly string s_MilitaryTableClassName = "military-pane-military-table";

        private World? _world;
        private Faction? _faction;
        private TabId _tab;

        private readonly IconFactory _iconFactory;

        public MilitaryPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new MultiTabGamePaneController(),
                  uiElementFactory.GetClass(s_ClassName), 
                  new TextUiElement(uiElementFactory.GetClass(s_TitleClassName), new ButtonController(), s_Title),
                  uiElementFactory.CreateSimpleButton(s_CloseClass).Item1,
                  TabBar<TabId>.Create(
                    new List<TabBar<TabId>.Definition>()
                    {
                        new(TabId.Army, "Army"),
                        new(TabId.Fleet, "Fleet")
                    },
                    uiElementFactory.GetClass(s_TabContainerClassName),
                    uiElementFactory.GetClass(s_TabOptionClassName)))
        {
            _iconFactory = iconFactory;
            var body = new
                DynamicUiContainer(
                    uiElementFactory.GetClass(s_BodyClassName), new NoOpElementController<UiContainer>());
            var formationTable =
                new DynamicKeyedTable<IFormation, FormationRow>(
                    uiElementFactory.GetClass(s_MilitaryTableClassName),
                    new ActionTableController(10f),
                    UiSerialContainer.Orientation.Vertical,
                    GetRange,
                    x => FormationRow.Create(x, uiElementFactory, _iconFactory),
                    Comparer<IFormation>.Create((x, y) => x.Name.CompareTo(y.Name)));
            body.Add(formationTable);
            SetBody(body);
        }

        public override void Populate(params object?[] args)
        {
            _world = args[0] as World;
            _faction = args[1] as Faction;
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }

        public override void SetTab(object id)
        {
            _tab = (TabId)id;
        }

        private IEnumerable<IFormation> GetRange()
        {
            if (_world == null || _faction == null)
            {
                return Enumerable.Empty<IFormation>();
            }
            return _tab switch
            {
                TabId.Army => Enumerable.Empty<IFormation>(),
                TabId.Fleet => _world.GetFleetsFor(_faction),
                _ => Enumerable.Empty<IFormation>(),
            };
        }
    }
}
