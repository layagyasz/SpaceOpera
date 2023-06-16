using SpaceOpera.Core.Military;

namespace SpaceOpera.View.Game.Info
{
    public class ShieldDescriber : IDescriber
    {
        public void DescribeAll(IEnumerable<object> objects, InfoPanel infoPanel)
        {
            infoPanel.AddValues("Absorption", objects.Cast<Shield>().Select(x => x.Absorption), "{0:0.##}");
            infoPanel.AddValues("Capacity", objects.Cast<Shield>().Select(x => x.Capacity), "{0:0.##}");
            infoPanel.AddValues("Recharge", objects.Cast<Shield>().Select(x => x.Recharge), "{0:0.####}");
        }

        public void Describe(object @object, InfoPanel infoPanel)
        {
            DescribeAll(Enumerable.Repeat(@object, 1), infoPanel);
        }
    }
}