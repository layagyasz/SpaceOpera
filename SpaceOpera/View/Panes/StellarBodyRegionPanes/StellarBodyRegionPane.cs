using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.View.Icons;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Controller.Panes;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.Panes.StellarBodyRegionPanes
{
    public class StellarBodyRegionPane : MultiTabGamePane
    {
        enum TabId
        {
            Structures
        }

        private static readonly string s_ClassName = "stellar-body-region-pane";
        private static readonly string s_TitleClassName = "stellar-body-region-pane-title";
        private static readonly string s_CloseClass = "stellar-body-region-pane-close";
        private static readonly string s_TabContainerClassName = "stellar-body-region-pane-tab-container";
        private static readonly string s_TabOptionClassName = "stellar-body-region-pane-tab-option";

        private World? _world;
        private Faction? _faction;
        private StellarBodyRegion? _region;
        private StellarBodyRegionHolding? _holding;
        private TabId _tab;

        private StructureTab _structureTab;

        public StellarBodyRegionPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new MultiTabGamePaneController(),
                  uiElementFactory.GetClass(s_ClassName),
                  new TextUiElement(uiElementFactory.GetClass(s_TitleClassName), new ButtonController(), string.Empty),
                  uiElementFactory.CreateSimpleButton(s_CloseClass).Item1,
                  TabBar<TabId>.Create(
                    new List<TabBar<TabId>.Definition>()
                    {
                        new(TabId.Structures, "Structures"),
                    },
                    uiElementFactory.GetClass(s_TabContainerClassName),
                    uiElementFactory.GetClass(s_TabOptionClassName)))
        {
            _structureTab = new(uiElementFactory, iconFactory);
            _structureTab.Initialize();
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
            _structureTab.Populate(_world, _holding);

            SetTitle(_region?.Name ?? "Unknown Region");
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }

        public override void SetTab(object id)
        {
            _tab = (TabId)id;
            switch (_tab)
            {
                case TabId.Structures:
                    SetBody(_structureTab);
                    break;
            }
        }
    }
}
