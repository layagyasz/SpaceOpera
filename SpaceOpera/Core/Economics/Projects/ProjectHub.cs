namespace SpaceOpera.Core.Economics.Projects
{
    public abstract class ProjectHub
    {
        private readonly List<IProject> _projects = new();

        public void AddProject(IProject project)
        {
            _projects.Add(project);
        }

        public void RemoveProject(IProject project)
        {
            _projects.Remove(project);
        }

        public IEnumerable<IProject> GetProjects()
        {
            return _projects;
        }
    }
}