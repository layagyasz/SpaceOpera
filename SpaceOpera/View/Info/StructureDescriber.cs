using SpaceOpera.Core.Economics;

namespace SpaceOpera.View.Info
{
    public class StructureDescriber : IDescriber
    {
        public void Describe(IEnumerable<object> objects, InfoPanel infoPanel)
        {
            throw new NotImplementedException();
        }

        public void Describe(object @object, InfoPanel infoPanel)
        {
            var structure = (Structure)@object;
            infoPanel.AddTitle(@object, structure.Name);
            infoPanel.AddQuantities("Cost", structure.Cost.GetQuantities());
        }
    }
}