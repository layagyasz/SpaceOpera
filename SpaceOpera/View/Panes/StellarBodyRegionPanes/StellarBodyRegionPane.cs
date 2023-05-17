using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.View.Icons;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Universe;
using SpaceOpera.Controller.Panes.StellarBodyRegionPanes;

namespace SpaceOpera.View.Panes.StellarBodyRegionPanes
{
    public class StellarBodyRegionPane : MultiTabGamePane
    {
        enum TabId
        {
            Structures
        }

        private static readonly string s_Container = "stellar-body-region-pane";
        private static readonly string s_Title = "stellar-body-region-pane-title";
        private static readonly string s_Close = "stellar-body-region-pane-close";
        private static readonly string s_TabContainer = "stellar-body-region-pane-tab-container";
        private static readonly string s_TabOption = "stellar-body-region-pane-tab-option";

        private World? _world;
        private Faction? _faction;
        private StellarBodyRegion? _region;
        private StellarBodyRegionHolding? _holding;
        private TabId _tab;

        public StructureTab StructureTab { get; }

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
                    },
                    uiElementFactory.GetClass(s_TabContainer),
                    uiElementFactory.GetClass(s_TabOption)))
        {
            StructureTab = new(uiElementFactory, iconFactory);
            StructureTab.Initialize();
        }

        public StellarBodyRegionHolding GetHolding()
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
                _holding = _world.Economy.GetHolding(_faction, _region);
            }
            StructureTab.Populate(_world, _holding);

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
            }
        }
    }
}
