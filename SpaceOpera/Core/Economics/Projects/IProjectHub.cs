namespace SpaceOpera.Core.Economics.Projects
{
    public interface IProjectHub
    {
        IEnumerable<IProject> GetProjects();
    }
}
