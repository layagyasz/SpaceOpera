using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.Info
{
    public class StarSystemDescriber : IDescriber
    {
        public void Describe(IEnumerable<object> objects, InfoPanel infoPanel)
        {
            throw new NotImplementedException();
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