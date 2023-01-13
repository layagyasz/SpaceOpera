namespace SpaceOpera.Core.Economics
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

        public IEnumerable<T> GetProjects<T>()
        {
            return _projects.Where(x => x is T).Cast<T>();
        }
    }
}