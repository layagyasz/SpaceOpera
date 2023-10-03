﻿using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.View.Icons;
using SpaceOpera.Controller.Game.Panes;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class DiplomacyPane : SimpleGamePane
    {
        private static readonly string s_Container = "diplomacy-pane";
        private static readonly string s_Title = "diplomacy-pane-title";
        private static readonly string s_Close = "diplomacy-pane-close";
        private static readonly string s_Body = "diplomacy-pane-body";

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private World? _world;
        private Faction? _faction;
        private DiplomaticRelation? _diplomaticRelation;

        public DiplomacyPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new GamePaneController(),
                  uiElementFactory.GetClass(s_Container),
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), "Diplomacy"),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1)
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            var body = new
                UiSerialContainer(
                    uiElementFactory.GetClass(s_Body), 
                    new NoOpElementController<UiSerialContainer>(), 
                    UiSerialContainer.Orientation.Horizontal);
            SetBody(body);
        }

        public override void Populate(params object?[] args)
        {
            _world = args[0] as World;
            _faction = args[1] as Faction;
            _diplomaticRelation = args[2] as DiplomaticRelation;
            SetTitle(_diplomaticRelation!.Faction.Name);
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }
    }
}
