﻿using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Game.Panes.StellarBodyRegionPanes;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.StellarBodyRegionPanes
{
    public class ProjectTab : DynamicUiSerialContainer
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

        public UiCompoundComponent Projects { get; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private ProjectHub? _projectHub;

        public ProjectTab(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                uiElementFactory.GetClass(s_Container),
                new ProjectTabController(),
                Orientation.Horizontal)
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            Projects =
                new DynamicUiCompoundComponent(
                    new ActionComponentController(),
                    new DynamicKeyedTable<IProject, ActionRow<IProject>>(
                        uiElementFactory.GetClass(s_ProjectTable),
                        new TableController(10f),
                        Orientation.Vertical,
                        GetRange,
                        CreateRow,
                        Comparer<IProject>.Create(
                            (x, y) => y.Progress.PercentFull().CompareTo(x.Progress.PercentFull()))));
            Add(Projects);
        }

        public void Populate(ProjectHub? projectHub)
        {
            _projectHub = projectHub;
        }

        private ActionRow<IProject> CreateRow(IProject project)
        {
            return ActionRow<IProject>.Create(
                project,
                ActionId.Unknown,
                _uiElementFactory,
                s_ProjectRowStyle,
                new List<IUiElement>()
                {
                    _iconFactory.Create(_uiElementFactory.GetClass(s_Icon), new InlayController(), project.Key),
                    new DynamicUiSerialContainer(
                        _uiElementFactory.GetClass(s_Info),
                        new NoOpElementController<UiSerialContainer>(),
                        Orientation.Vertical)
                    {
                        new TextUiElement(
                            _uiElementFactory.GetClass(s_Text), new InlayController(), project.Name),
                        new DynamicUiSerialContainer(
                            _uiElementFactory.GetClass(s_Status),
                            new NoOpElementController<UiSerialContainer>(),
                            Orientation.Vertical)
                        {
                            new DynamicTextUiElement(
                                _uiElementFactory.GetClass(s_StatusText),
                                new InlayController(),
                                () => GetStatusString(project)),
                            new PoolBar(
                                _uiElementFactory.GetClass(s_StatusProgress),
                                new InlayController(),
                                project.Progress),
                        }
                    }
                },
                s_ProjectActions);
        }

        private IEnumerable<IProject> GetRange()
        {
            if (_projectHub == null)
            {
                return Enumerable.Empty<IProject>();
            }
            return _projectHub.GetProjects();
        }

        private static string GetStatusString(IProject project)
        {
            return $"{project.Progress.ToString("N0")} - {EnumMapper.ToString(project.Status)}";
        }
    }
}