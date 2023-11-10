using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Game.Panes.StellarBodyRegionPanes;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.StellarBodyRegionPanes
{
    public class ProjectTab : DynamicUiCompoundComponent
    {
        private static readonly string s_Container = "stellar-body-region-pane-body";

        private static readonly string s_ProjectTable = "stellar-body-region-pane-project-table";
        private static readonly ActionRow<IProject>.Style s_ProjectRowStyle =
            new()
            {
                Container = "stellar-body-region-pane-project-row",
                ActionContainer = "stellar-body-region-pane-project-row-action-container"
            };
        private static readonly string s_Icon = "stellar-body-region-pane-project-row-icon";
        private static readonly string s_Info = "stellar-body-region-pane-project-row-info";
        private static readonly string s_Text = "stellar-body-region-pane-project-row-text";
        private static readonly string s_Status = "stellar-body-region-pane-project-row-status-container";
        private static readonly string s_StatusText = "stellar-body-region-pane-project-row-status-text";
        private static readonly string s_StatusProgress = "stellar-body-region-pane-project-row-status-progress";
        private static readonly List<ActionRow<IProject>.ActionConfiguration> s_ProjectActions =
            new()
            {
                new ()
                {
                    Button = "stellar-body-region-pane-project-row-action-cancel",
                    Action = ActionId.Cancel
                }
            };

        class ProjectRange : IRange<IProject>
        {
            public ProjectHub? ProjectHub { get; set; }

            public IEnumerable<IProject> GetRange()
            {
                return ProjectHub?.GetProjects() ?? Enumerable.Empty<IProject>();
            }
        }

        public UiCompoundComponent Projects { get; }

        private readonly ProjectRange _range = new();

        public ProjectTab(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new ProjectTabController(), 
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new NoOpElementController(),
                      UiSerialContainer.Orientation.Horizontal))
        {
            Projects =
                new DynamicUiCompoundComponent(
                    new ActionComponentController(),
                    new DynamicKeyedTable<IProject>(
                        uiElementFactory.GetClass(s_ProjectTable),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        _range,
                        new SimpleKeyedElementFactory<IProject>(uiElementFactory, iconFactory, CreateRow),
                        Comparer<IProject>.Create(
                            (x, y) => y.Progress.PercentFull().CompareTo(x.Progress.PercentFull()))));
            Add(Projects);
        }

        public void Populate(ProjectHub? projectHub)
        {
            _range.ProjectHub = projectHub;
        }

        private static ActionRow<IProject> CreateRow(
            IProject project, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return ActionRow<IProject>.Create(
                project,
                ActionId.Unknown,
                ActionId.Unknown,
                uiElementFactory,
                s_ProjectRowStyle,
                new List<IUiElement>()
                {
                    iconFactory.Create(uiElementFactory.GetClass(s_Icon), new InlayController(), project.Key),
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_Info),
                        new NoOpElementController(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        new TextUiElement(
                            uiElementFactory.GetClass(s_Text), new InlayController(), project.Name),
                        new DynamicUiSerialContainer(
                            uiElementFactory.GetClass(s_Status),
                            new NoOpElementController(),
                            UiSerialContainer.Orientation.Vertical)
                        {
                            new DynamicTextUiElement(
                                uiElementFactory.GetClass(s_StatusText),
                                new InlayController(),
                                () => GetStatusString(project)),
                            new PoolBar(
                                uiElementFactory.GetClass(s_StatusProgress),
                                new InlayController(),
                                project.Progress),
                        }
                    }
                },
                s_ProjectActions);
        }

        private static string GetStatusString(IProject project)
        {
            return $"{project.Progress.ToString("N0")} - {EnumMapper.ToString(project.Status)}";
        }
    }
}
