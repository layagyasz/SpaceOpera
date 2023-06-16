using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.Game.Info
{
    public class StarSystemDescriber : IDescriber
    {
        public void DescribeAll(IEnumerable<object> objects, InfoPanel infoPanel)
        {
            throw new NotSupportedException();
        }

        public void Describe(object @object, InfoPanel infoPanel)
        {
            var starSystem = (StarSystem)@object;
            infoPanel.AddHeader(starSystem.Name);
            foreach (var group in starSystem.Orbiters.GroupBy(x => x.Type).OrderBy(x => x.Key))
            {
                infoPanel.AddValue(group.Key, group.Count().ToString());
            }
        }
    }
}