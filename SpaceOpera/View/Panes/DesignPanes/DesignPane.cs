﻿using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Panes;
using SpaceOpera.Core;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;

namespace SpaceOpera.View.Panes.DesignPanes
{
    public class DesignPane : GamePane
    {
        private static readonly string s_Title = "Designs";

        private static readonly string s_ClassName = "design-pane";
        private static readonly string s_TitleClassName = "design-pane-title";
        private static readonly string s_CloseClass = "design-pane-close";
        private static readonly string s_TabContainerClassName = "design-pane-tab-container";
        private static readonly string s_TabOptionClassName = "design-pane-tab-option";
        private static readonly string s_BodyClassName = "design-pane-body";
        private static readonly string s_DesignTableClassName = "design-pane-design-table";

        private World? _world;
        private Faction? _faction;
        private ComponentType _componentType;

        public DesignPane(UiElementFactory uiElementFactory)
            : base(
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
                uiElementFactory.GetClass(s_TabOptionClassName)),
                new DynamicUiContainer(
                    uiElementFactory.GetClass(s_BodyClassName), new NoOpElementController<UiContainer>())) 
        {
            var table =
                new DynamicUiSerialContainer<Design, DesignRow>(
                    uiElementFactory.GetClass(s_DesignTableClassName), 
                    new TableController(),
                    UiSerialContainer.Orientation.Vertical,
                    GetRange,
                    x => DesignRow.Create(x, uiElementFactory),
                    Comparer<Design>.Create((x, y) => x.Name.CompareTo(y.Name)));
            AddToBody(table);
        }

        public override void Populate(params object?[] args)
        {
            _world = args[0] as World;
            _faction = args[1] as Faction;
            Refresh();
        }

        public override void SetTab(object id)
        {
            _componentType = (ComponentType)id;
        }

        private IEnumerable<Design> GetRange()
        {
            if (_world == null || _faction == null)
            {
                return Enumerable.Empty<Design>();
            }
            return _world.GetDesignsFor(_faction).Where(x => x.Configuration.Template.Type == _componentType);
        }
    }
}
