using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Economics
{
    abstract class ProjectHub
    {
        private readonly List<IProject> _Projects = new List<IProject>();

        public void AddProject(IProject Project)
        {
            _Projects.Add(Project);
        }

        public void RemoveProject(IProject Project)
        {
            _Projects.Remove(Project);
        }

        public IEnumerable<T> GetProjects<T>()
        {
            return _Projects.Where(x => x is T).Cast<T>();
        }
    }
}