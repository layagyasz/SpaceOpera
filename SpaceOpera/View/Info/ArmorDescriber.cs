using SpaceOpera.Core.Military;

namespace SpaceOpera.View.Info
{
    public class ArmorDescriber : IDescriber
    {
        public void DescribeAll(IEnumerable<object> objects, InfoPanel infoPanel)
        {
            infoPanel.AddValues("Coverage", objects.Cast<Armor>().Select(x => x.Coverage), "{0:P0}");
            infoPanel.AddValues("Thickness", objects.Cast<Armor>().Select(x => x.Thickness), "{0:0.##}");
            infoPanel.AddValues("Protection", objects.Cast<Armor>().Select(x => x.Protection), "{0:0.##}");
        }

        public void Describe(object @object, InfoPanel infoPanel)
        {
            DescribeAll(Enumerable.Repeat(@object, 1), infoPanel);
        }
    }
}