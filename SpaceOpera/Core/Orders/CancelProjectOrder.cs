using SpaceOpera.Core.Economics.Projects;

namespace SpaceOpera.Core.Orders
{
    public class CancelProjectOrder : IOrder
    {
        public IProject Project { get; }

        public CancelProjectOrder(IProject project)
        {
            Project = project;
        }

        public ValidationFailureReason Validate(World world)
        {
            return ValidationFailureReason.None;
        }

        public bool Execute(World World)
        {
            World.ProjectManager.Cancel(Project);
            return true;
        }
    }
}
