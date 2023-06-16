using SpaceOpera.Core.Economics;

namespace SpaceOpera.View.Game.Info
{
    public class StructureDescriber : IDescriber
    {
        public void DescribeAll(IEnumerable<object> objects, InfoPanel infoPanel)
        {
            throw new NotSupportedException();
        }

        public void Describe(object @object, InfoPanel infoPanel)
        {
            var structure = (Structure)@object;
            infoPanel.AddTitle(@object, structure.Name);
            infoPanel.AddQuantities("Cost", structure.Cost.GetQuantities());
        }
    }
}