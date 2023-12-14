using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Components.Dynamics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.FormationPanes
{
    public class FormationComponentHeader 
        : DynamicUiCompoundComponent, IKeyedUiElement<IFormationDriver>, IActionRow
    {
        private static readonly ActionRowStyles.Style s_HeaderStyle =
            new()
            {
                Container = "formation-pane-formation-header",
                ActionContainer = "formation-pane-formation-header-action-container"
            };
        private static readonly string s_Icon = "formation-pane-formation-header-icon";
        private static readonly string s_Info = "formation-pane-formation-header-info";
        private static readonly string s_NameContainer = "formation-pane-formation-header-name-container";
        private static readonly string s_Name = "formation-pane-formation-header-name";
        private static readonly ChipSetStyles.ChipStyle s_MilitaryPower = new()
        {
            Container = "formation-pane-formation-header-military-power",
            Icon = "formation-pane-formation-header-military-power-icon",
            Text = "formation-pane-formation-header-military-power-text"
        };
        private static readonly string s_CurrentAction = "formation-pane-formation-header-current-action";
        private static readonly string s_HealthText = "formation-pane-formation-header-health-text";
        private static readonly string s_Health = "formation-pane-formation-header-health";
        private static readonly string s_CohesionText = "formation-pane-formation-header-cohesion-text";
        private static readonly string s_Cohesion = "formation-pane-formation-header-cohesion";
        private static readonly string s_AssignmentContainer = "formation-pane-formation-header-assignment-container";

        private static readonly List<ActionRowStyles.ActionConfiguration> s_ArmyAssignments =
            new()
            {
                new()
                {
                    Button = "formation-pane-formation-header-assignment-none",
                    Action = ActionId.NoAssignment
                },
                new()
                {
                    Button = "formation-pane-formation-header-assignment-defend",
                    Action = ActionId.Defend
                },
                new()
                {
                    Button = "formation-pane-formation-header-assignment-train",
                    Action = ActionId.Train
                }
            };
        private static readonly List<ActionRowStyles.ActionConfiguration> s_FleetAssignments =
            new()
            {
                new()
                {
                    Button = "formation-pane-formation-header-assignment-none",
                    Action = ActionId.NoAssignment
                },
                new()
                {
                    Button = "formation-pane-formation-header-assignment-logistics",
                    Action = ActionId.Logistics
                },
                new()
                {
                    Button = "formation-pane-formation-header-assignment-patrol",
                    Action = ActionId.Patrol
                }
            };
        private static readonly List<ActionRowStyles.ActionConfiguration> s_DivisionAssignments =
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
                },
                new()
                {
                    Button = "formation-pane-formation-header-assignment-train",
                    Action = ActionId.Train
                }
            };

        private static readonly List<ActionRowStyles.ActionConfiguration> s_HeaderActions =
            new()
            {
                new ()
                {
                    Button = "formation-pane-formation-header-action-close",
                    Action = ActionId.Unselect
                }
            };

        public EventHandler<ElementEventArgs>? ActionAdded { get; set; }
        public EventHandler<ElementEventArgs>? ActionRemoved { get; set; }

        public IFormationDriver Key { get; }
        private readonly List<IUiElement> _actions = new();

        public FormationComponentHeader(
            IFormationDriver driver, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                new ActionRowController<IFormationDriver>(driver, ActionId.Unknown, ActionId.Unknown),
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_HeaderStyle.Container!),
                    new ButtonController(), 
                    UiSerialContainer.Orientation.Horizontal))
        {
            Key = driver;
            Add(iconFactory.Create(uiElementFactory.GetClass(s_Icon), new InlayController(), driver));

            var info =
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Info), new InlayController(), UiSerialContainer.Orientation.Vertical)
                {
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_NameContainer), 
                        new InlayController(),
                        UiSerialContainer.Orientation.Horizontal)
                    {
                        new TextUiElement(
                            uiElementFactory.GetClass(s_Name), new InlayController(), driver.Formation.Name),
                        MilitaryPowerChip.Create(driver.Formation.GetMilitaryPower, s_MilitaryPower, uiElementFactory)
                    },
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_CurrentAction), new InlayController(), GetCurrentAction)
                };
            if (driver is AtomicFormationDriver atomicFormation)
            { 
                info.Add(
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_HealthText),
                        new InlayController(),
                        () => atomicFormation.AtomicFormation.Health.ToString("N0")));
                info.Add(
                    new PoolBar(
                        uiElementFactory.GetClass(s_Health),
                        new InlayController(),
                        atomicFormation.AtomicFormation.Health));
                info.Add(
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_CohesionText),
                        new InlayController(),
                        () => string.Format("{0:P0}", atomicFormation.AtomicFormation.Cohesion.PercentFull())));
                info.Add(
                    new PoolBar(
                        uiElementFactory.GetClass(s_Cohesion),
                        new InlayController(),
                        atomicFormation.AtomicFormation.Cohesion));
            }
            Add(info);

            var assignments = 
                new DynamicUiCompoundComponent(
                    new DynamicRadioController<ActionId>(GetAssignment),
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_AssignmentContainer),
                        new InlayController(),
                        UiSerialContainer.Orientation.Horizontal));
            foreach (var assignment in GetAssignments(driver))
            {
                var a =
                    new SimpleUiElement(
                        uiElementFactory.GetClass(assignment.Button!),
                        new ActionButtonController(assignment.Action));
                assignments.Add(a);
                _actions.Add(a);
            }
            Add(assignments);

            foreach (var action in s_HeaderActions)
            {
                var wrapper =
                    new UiWrapper(
                        uiElementFactory.GetClass(s_HeaderStyle.ActionContainer!),
                        new ButtonController(),
                        new SimpleUiElement(
                            uiElementFactory.GetClass(action.Button!),
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
            if (Key is ArmyDriver)
            {
                return string.Empty;
            }
            var action = ((AtomicFormationDriver)Key).GetCurrentAction();
            if (action is AttackAction)
            {
                return "Attacking";
            }
            if (action is CombatAction)
            {
                return "In combat";
            }
            if (action is DefendAction)
            {
                return "Defending";
            }
            if (action is EngageAction engage)
            {
                return "Engaging " + engage.Target.Name;
            }
            if (action is IdleAction)
            {
                return "Waiting";
            }
            if (action is LoadAction)
            {
                return "Loading";
            }
            if (action is MoveAction move)
            {
                return "Moving to " + move.Movement.Destination.Name;
            }
            if (action is RegroupAction)
            {
                return "Regrouping";
            }
            if (action is RetreatAction)
            {
                return "Retreating";
            }
            if (action is SpotAction spot)
            {
                return "Spotting " + spot.Target.Name;
            }
            if (action is TrainAction)
            {
                return "Training";
            }
            if (action is UnloadAction)
            {
                return "Unload";
            }
            return action.GetType().ToString();
        }

        private static IEnumerable<ActionRowStyles.ActionConfiguration> GetAssignments(IFormationDriver driver)
        {
            if (driver is ArmyDriver)
            {
                return s_ArmyAssignments;
            }
            if (driver is FleetDriver)
            {
                return s_FleetAssignments;
            }
            if (driver is DivisionDriver)
            {
                return s_DivisionAssignments;
            }
            return Enumerable.Empty<ActionRowStyles.ActionConfiguration>();
        }
    }
}
