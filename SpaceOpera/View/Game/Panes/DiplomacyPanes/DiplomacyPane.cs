using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.View.Icons;
using SpaceOpera.Controller.Game.Panes.DiplomacyPanes;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class DiplomacyPane : SimpleGamePane
    {
        private static readonly string s_Container = "diplomacy-pane";
        private static readonly string s_Title = "diplomacy-pane-title";
        private static readonly string s_Close = "diplomacy-pane-close";

        public DiplomacyComponent Diplomacy { get; }

        private World? _world;
        private Faction? _faction;
        private DiplomaticRelation? _diplomaticRelation;

        public DiplomacyPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new DiplomacyPaneController(),
                  uiElementFactory.GetClass(s_Container),
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), string.Empty),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1)
        {
            Diplomacy = new DiplomacyComponent(uiElementFactory);
            SetBody(Diplomacy);
        }

        public override void Populate(params object?[] args)
        {
            _world = args[0] as World;
            _faction = args[1] as Faction;
            _diplomaticRelation = args[2] as DiplomaticRelation;
            SetTitle(_diplomaticRelation!.Faction.Name);
            Diplomacy.Populate(_world!, _diplomaticRelation);
            Populated?.Invoke(this, EventArgs.Empty);
        }
    }
}
