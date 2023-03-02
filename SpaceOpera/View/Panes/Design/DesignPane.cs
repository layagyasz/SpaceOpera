﻿using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Panes;
using SpaceOpera.Core.Designs;

namespace SpaceOpera.View.Panes.Design
{
    public class DesignPane : GamePane
    {
        private static readonly string s_Title = "Designs";

        private static readonly string s_ClassName = "pane-standard";
        private static readonly string s_TitleClassName = "pane-standard-title";
        private static readonly string s_CloseClass = "pane-standard-close";
        private static readonly string s_TabContainerClassName = "pane-tab-container";
        private static readonly string s_TabOptionClassName = "pane-tab-option";

        public DesignPane(
            IElementController controller, Class @class, IUiElement header, IUiElement closeButton, UiComponent tabs)
            : base(controller, @class, header, closeButton, tabs) { }

        public override void SetTab(object id) { }

        public static DesignPane Create(UiElementFactory uiElementFactory)
        {
            return new(
                new GamePaneController(),
                uiElementFactory.GetClass(s_ClassName),
                uiElementFactory.CreateTextButton(s_TitleClassName, s_Title).Item1, 
                uiElementFactory.CreateTextButton(s_CloseClass, "X").Item1,
                TabBar<ComponentType>.Create(
                    new List<TabBar<ComponentType>.Definition>()
                    {
                        new(ComponentType.Ship, "Ship"),
                        new(ComponentType.ShipWeapon, "Ship Weapon"),
                        new(ComponentType.ShipShield, "Ship Shield")
                    },
                    uiElementFactory.GetClass(s_TabContainerClassName),
                    uiElementFactory.GetClass(s_TabOptionClassName)));
        }
    }
}
