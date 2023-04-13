using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Panes.FormationPanes;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.FormationPanes
{
    public class FormationPane : DynamicUiSerialContainer, IGamePane
    {
        private static readonly string s_ClassName = "formation-pane";
        private static readonly string s_FormationListClassName = "formation-pane-formation-list";

        public EventHandler<EventArgs>? Populated { get; set; }

        public UiCompoundComponent FormationList { get; set; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        public FormationPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(uiElementFactory.GetClass(s_ClassName), new FormationPaneController(), Orientation.Vertical)
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            FormationList =
                new DynamicUiCompoundComponent(
                    new FormationListController(),
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_FormationListClassName),
                        new TableController(10f),
                        Orientation.Vertical));
            Add(FormationList);
        }

        public void Populate(params object?[] args)
        {
            FormationList.Clear(/* dispose= */ true);
            foreach (var driver in ((IEnumerable<object>)args[0]!).Cast<IFormationDriver>())
            {
                var component = new FormationComponent(driver, _uiElementFactory, _iconFactory);
                component.Initialize();
                FormationList.Add(component);
            }
            Refresh();
        }
    }
}
