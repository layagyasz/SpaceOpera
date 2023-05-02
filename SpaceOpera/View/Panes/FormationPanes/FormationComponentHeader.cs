using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Military.Ai.Actions;
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
        private static readonly string s_Info = "formation-pane-formation-header-info";
        private static readonly string s_Name = "formation-pane-formation-header-name";
        private static readonly string s_CurrentAction = "formation-pane-formation-header-current-action";
        private static readonly string s_AssignmentContainer = "formation-pane-formation-header-assignment-container";

        private static readonly List<ActionRow<FormationDriver>.ActionConfiguration> s_FleetAssignments =
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
        private static readonly List<ActionRow<FormationDriver>.ActionConfiguration> s_DivisionAssignments =
            new()
            {
                new()
                {
                    Button = "formation-pane-formation-header-assignment-none",
                    Action = ActionId.NoAssignment
                },
                new()
                {
                    Button = "formation-pane-formation-header-assignment-move",
                    Action = ActionId.Move
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
            Add(
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Info), new InlayController(), UiSerialContainer.Orientation.Vertical)
                { 
                    new TextUiElement(uiElementFactory.GetClass(s_Name), new InlayController(), driver.Formation.Name),
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_CurrentAction), new InlayController(), GetCurrentAction)
                });

            var assignments = 
                new DynamicUiCompoundComponent(
                    new DynamicRadioController<ActionId>(
                        "assignment-" + GetHashCode(), GetAssignment),
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_AssignmentContainer),
                        new InlayController(),
                        UiSerialContainer.Orientation.Horizontal));
            foreach (var assignment in GetAssignments(driver))
            {
                var a =
                    new SimpleUiElement(
                        uiElementFactory.GetClass(assignment.Button),
                        new ActionButtonController(assignment.Action));
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

        private ActionId GetAssignment()
        {
            return ActionIdMapper.ToActionId(Key.GetAssignment());
        }

        private string GetCurrentAction()
        {
            var action = Key.GetCurrentAction();
            if (action == null)
            {
                return "Waiting";
            }
            var type = action.GetType();
            if (type == typeof(CombatAction))
            {
                return "In combat";
            }
            if (type == typeof(EngageAction))
            {
                return "Engaging " + ((EngageAction)action).Target.Name;
            }
            if (type == typeof(IdleAction))
            {
                return "Awaiting orders";
            }
            if (type == typeof(MoveAction))
            {
                return "Moving to " + ((MoveAction)action).Movement.Destination.Name;
            }
            if (type == typeof(RegroupAction))
            {
                return "Regrouping";
            }
            if (type == typeof(SpotAction))
            {
                return "Spotting " + ((SpotAction)action).Target.Name;
            }
            return type.ToString();
        }

        private static IEnumerable<ActionRow<FormationDriver>.ActionConfiguration> GetAssignments(
            FormationDriver driver)
        {
            if (driver is FleetDriver)
            {
                return s_FleetAssignments;
            }
            if (driver is DivisionDriver)
            {
                return s_DivisionAssignments;
            }
            return Enumerable.Empty<ActionRow<FormationDriver>.ActionConfiguration>();
        }
    }
}
