﻿using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes.FormationPanes;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.FormationPanes
{
    public class FormationPane : DynamicUiSerialContainer, IGamePane
    {
        private static readonly string s_Container = "formation-pane";
        private static readonly string s_FormationList = "formation-pane-formation-list";

        public EventHandler<EventArgs>? Populated { get; set; }

        public UiCompoundComponent FormationList { get; set; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        public FormationPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(uiElementFactory.GetClass(s_Container), new FormationPaneController(), Orientation.Vertical)
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            FormationList =
                new DynamicUiCompoundComponent(
                    new FormationListController(),
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_FormationList),
                        new TableController(10f),
                        Orientation.Vertical));
            Add(FormationList);
        }

        public void Populate(params object?[] args)
        {
            FormationList.Clear(/* dispose= */ true);
            foreach (var driver in (IEnumerable<object>)args[0]!)
            {
                IUiElement component;
                if (driver is AtomicFormationDriver atomicDriver)
                {
                    component = new FormationComponent(atomicDriver, _uiElementFactory, _iconFactory);
                }
                else if (driver is ArmyDriver armyDriver)
                {
                    component = new ArmyComponent(armyDriver, _uiElementFactory, _iconFactory);
                }
                else
                {
                    throw new ArgumentException($"Unsupported object type {driver.GetType()}.");
                }
                component.Initialize();
                FormationList.Add(component);
            }
            Refresh();
        }
    }
}
