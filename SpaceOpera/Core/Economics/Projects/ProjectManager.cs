namespace SpaceOpera.Core.Economics.Projects
{
    public class ProjectManager
    {
        private List<IProject> _projects = new();
        private List<IProject> _buffer = new();

        public void Add(IProject project)
        {
            project.Setup();
            _projects.Add(project);
        }

        public void Cancel(IProject project)
        {
            _projects.Remove(project);
            project.Cancel();
        }

        public void Tick(World world)
        {
            foreach (var project in _projects)
            {
                project.Tick();
                if (project.Status == ProjectStatus.Done)
                {
                    project.Finish(world);
                }
                else
                {
                    _buffer.Add(project);
                }
            }
            (_projects, _buffer) = (_buffer, _projects);
            _buffer.Clear();
        }
    }
}
