using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.FormationPanes
{
    public class FormationComponentHeader 
        : DynamicUiCompoundComponent, IKeyedUiElement<FormationDriver>, IActionRow
    {
        private static readonly ActionRow<FormationDriver>.Style s_HeaderStyle =
            new()
            {
                Container = "formation-pane-formation-header",
                ActionContainer = "formation-pane-formation-header-action-container"
            };
        private static readonly string s_Icon = "formation-pane-formation-header-icon";
        private static readonly string s_Text = "formation-pane-formation-header-text";
        private static readonly string s_AssignmentContainer = "formation-pane-formation-header-assignment-container";
        private static readonly List<ActionRow<FormationDriver>.ActionConfiguration> s_HeaderAssignments =
            new()
            {
                new()
                {
                    Button = "formation-pane-formation-header-assignment-none",
                    Action = ActionId.NoAssignment
                },
                new()
                {
                    Button = "formation-pane-formation-header-assignment-patrol",
                    Action = ActionId.Patrol
                }
            };
        private static readonly List<ActionRow<FormationDriver>.ActionConfiguration> s_HeaderActions =
            new()
            {
                new ()
                {
                    Button = "formation-pane-formation-header-action-close",
                    Action = ActionId.Unselect
                }
            };

        public FormationDriver Key { get; }
        private readonly List<IUiElement> _actions = new();

        public FormationComponentHeader(
            FormationDriver driver, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                new ActionRowController<FormationDriver>(driver),
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_HeaderStyle.Container),
                    new ButtonController(), 
                    UiSerialContainer.Orientation.Horizontal))
        {
            Key = driver;
            Add(iconFactory.Create(uiElementFactory.GetClass(s_Icon), new InlayController(), driver));
            Add(new TextUiElement(
                    uiElementFactory.GetClass(s_Text), new InlayController(), driver.Formation.Name));

            var assignments = 
                new UiCompoundComponent(
                    new RadioController<ActionId>(
                        "assignment-" + GetHashCode(), ActionIdMapper.ToActionId(driver.GetAssignment())),
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_AssignmentContainer),
                        new InlayController(),
                        UiSerialContainer.Orientation.Horizontal));
            foreach (var assignment in s_HeaderAssignments)
            {
                var a = 
                    new SimpleUiElement(
                        uiElementFactory.GetClass(assignment.Button), new ActionButtonController(assignment.Action));
                assignments.Add(a);
                _actions.Add(a);
            }
            Add(assignments);

            foreach (var action in s_HeaderActions)
            {
                var wrapper =
                    new UiWrapper(
                        uiElementFactory.GetClass(s_HeaderStyle.ActionContainer),
                        new ButtonController(),
                        new SimpleUiElement(
                            uiElementFactory.GetClass(action.Button),
                            new ActionButtonController(action.Action)));
                Add(wrapper);
                _actions.AddRange(wrapper);
            }
        }

        public IEnumerable<IUiElement> GetActions()
        {
            return _actions;
        }
    }
}
