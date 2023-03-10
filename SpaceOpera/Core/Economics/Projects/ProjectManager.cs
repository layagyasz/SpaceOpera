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

        public void Tick()
        {
            foreach (var project in _projects)
            {
                project.Tick();
                if (project.IsDone())
                {
                    project.Finish();
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
