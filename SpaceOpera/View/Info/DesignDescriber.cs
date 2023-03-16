using SpaceOpera.Core.Designs;

namespace SpaceOpera.View.Info
{
    public class DesignDescriber : IDescriber
    {
        public void DescribeAll(IEnumerable<object> objects, InfoPanel infoPanel)
        {
            throw new NotSupportedException();
        }

        public void Describe(object @object, InfoPanel infoPanel)
        {
            var design = (Design)@object;
            new ComponentDescriber(true).DescribeAll(design.Components, infoPanel);
        }
    }
}
