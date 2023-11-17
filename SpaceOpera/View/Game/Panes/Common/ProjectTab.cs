using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes.Common;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.Common
{
    public class ProjectTab : DynamicUiCompoundComponent
    {
        public ProjectsComponent Projects { get; }

        public ProjectTab(
            Class @class, ProjectsComponent.Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new ProjectTabController(),
                  new DynamicUiSerialContainer(
                      @class,
                      new NoOpElementController(),
                      UiSerialContainer.Orientation.Horizontal))
        {
            Projects = ProjectsComponent.Create(style, uiElementFactory, iconFactory);
            Add(Projects);
        }

        public void SetProjectHub(IProjectHub? projectHub)
        {
            Projects.SetProjectHub(projectHub);
        }
    }
}
