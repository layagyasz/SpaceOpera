using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Core;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Game.Panes.Common;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.MilitaryPanes
{
    public class RecruitmentTab : DynamicUiCompoundComponent
    {
        private static readonly string s_Container = "military-pane-body";
        private static readonly string s_TemplateContainer = "military-pane-recruitment-template-container";
        private static readonly ActionRowStyles.Style s_TemplateHeaderStyle =
            new()
            {
                Container = "military-pane-recruitment-template-header"
            };
        private static readonly string s_TemplateHeaderSpace = 
            "military-pane-recruitment-template-header-select-wrapper";
        private static readonly Select.Style s_SelectStyle = new()
        {
            Root = "military-pane-recruitment-template-header-select",
            OptionContainer = "military-pane-recruitment-template-header-select-option-container",
            Option = "military-pane-recruitment-template-header-select-option"
        };

        private static readonly string s_TemplateTable = "military-pane-recruitment-template-table";
        private static readonly ActionRowStyles.Style s_TemplateRowStyle =
            new()
            {
                Container = "military-pane-recruitment-template-row",
                ActionContainer = "military-pane-recruitment-template-row-action-container"
            };
        private static readonly string s_Icon = "military-pane-recruitment-template-row-icon";
        private static readonly string s_Text = "military-pane-recruitment-template-row-text";

        private static readonly ProjectsComponent.Style s_ProjectsStyle = new()
        {
            Container = "military-pane-recruitment-project-table",
            RowContainer = new()
            {
                Container = "military-pane-recruitment-project-row",
                ActionContainer = "military-pane-recruitment-project-row-action-container"
            },
            Icon = "military-pane-recruitment-project-row-icon",
            Info = "military-pane-recruitment-project-row-info",
            Text = "military-pane-recruitment-project-row-text",
            Status = "military-pane-recruitment-project-row-status-container",
            StatusText = "military-pane-recruitment-project-row-status-text",
            StatusProgress = "military-pane-recruitment-project-row-status-progress",
            Cancel = "military-pane-recruitment-project-row-action-cancel"
        };

        class ProjectRange
        {
            public World? World { get; set; }
            public Faction? Faction { get; set; }

            public IEnumerable<IProject> GetRange()
            {
                if (World == null || Faction == null)
                {
                    return Enumerable.Empty<IProject>();
                }
                return World.Projects.GetFor(Faction).Where(x => x is CreateDivisionProject);
            }
        }

        public EventHandler<EventArgs>? Populated { get; set; }

        public IUiComponent Holdings { get; }
        public ActionTable<DivisionTemplate> Templates { get; }
        public ProjectsComponent Projects { get; }

        private StaticRange<DivisionTemplate> _templateRange;
        private ProjectRange _projectRange;

        private RecruitmentTab(
            Class @class,
            IUiComponent holdings, 
            ActionTable<DivisionTemplate> templates,
            StaticRange<DivisionTemplate> templateRange,
            ProjectsComponent projects,
            ProjectRange projectRange)
            : base(
                  new NoOpController(), 
                  new DynamicUiSerialContainer(@class, new NoOpElementController(), 
                      UiSerialContainer.Orientation.Horizontal))
        {
            Holdings = holdings;
            Templates = templates;
            Projects = projects;

            _templateRange = templateRange;
            _projectRange = projectRange;

            Add(Templates);
            Add(Projects);
        }

        public static RecruitmentTab Create(UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            var templateRange = new StaticRange<DivisionTemplate>();
            var projectRange = new ProjectRange();
            var holdings =
                uiElementFactory.CreateSelect(
                    s_SelectStyle,
                    Enumerable.Empty<SelectOption<EconomicSubzoneHolding>>(),
                    scrollSpeed: 10f).Item1;
            var projects = ProjectsComponent.Create(s_ProjectsStyle, uiElementFactory, iconFactory);
            return new(
                uiElementFactory.GetClass(s_Container),
                holdings,
                new ActionTable<DivisionTemplate>(
                    uiElementFactory.GetClass(s_TemplateContainer),
                    ActionRow<Type>.Create(
                        typeof(IFormationDriver),
                        ActionId.Unknown,
                        ActionId.Unknown,
                        uiElementFactory,
                        s_TemplateHeaderStyle,
                        new List<IUiElement>()
                        {
                            new UiWrapper(
                                uiElementFactory.GetClass(s_TemplateHeaderSpace), new InlayController(), holdings)
                        },
                        Enumerable.Empty<ActionRowStyles.ActionConfiguration>()),
                    DynamicKeyedContainer<DivisionTemplate>.CreateSerial(
                        uiElementFactory.GetClass(s_TemplateTable),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        templateRange.GetRange,
                        new SimpleKeyedElementFactory<DivisionTemplate>(uiElementFactory, iconFactory, CreateRow),
                        Comparer<DivisionTemplate>.Create((x, y) => x.Name.CompareTo(y.Name)))),
                templateRange,
                projects,
                projectRange);
        }


        public Faction GetFaction()
        {
            return _projectRange.Faction!;
        }

        public World GetWorld()
        {
            return _projectRange.World!;
        }

        public void Populate(World? world, Faction? faction)
        {
            if (world == null || faction == null)
            {
                _templateRange.Clear();
            }
            else
            {
                _templateRange.Set(
                    world.GetDesignsFor(faction)
                        .Where(x => x.Configuration.Template.Type == ComponentType.DivisionTemplate)
                        .SelectMany(x => x.Components)
                        .Cast<DivisionTemplate>());
            }
            _projectRange.World = world;
            _projectRange.Faction = faction;

            Populated?.Invoke(this, EventArgs.Empty);
        }

        private static IKeyedUiElement<DivisionTemplate> CreateRow(
            DivisionTemplate template, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return ActionRow<DivisionTemplate>.Create(
                template,
                ActionId.Add,
                ActionId.Unknown,
                uiElementFactory,
                s_TemplateRowStyle,
                new List<IUiElement>()
                {
                    iconFactory.Create(uiElementFactory.GetClass(s_Icon), new InlayController(), template),
                    new TextUiElement(
                        uiElementFactory.GetClass(s_Text), new InlayController(), template.Name)
                },
                Enumerable.Empty<ActionRowStyles.ActionConfiguration>());
        }
    }
}
