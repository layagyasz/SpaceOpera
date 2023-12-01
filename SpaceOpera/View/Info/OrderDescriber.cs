using SpaceOpera.Core.Orders.Economics;

namespace SpaceOpera.View.Info
{
    public class OrderDescriber : IDescriber
    {
        class BuildOrderDescriber : IDescriber
        {
            public void DescribeAll(IEnumerable<object> objects, InfoPanel infoPanel)
            {
                Describe(objects.First(), infoPanel);
            }

            public void Describe(object @object, InfoPanel infoPanel)
            {
                BuildOrder order = (BuildOrder)@object;
                infoPanel.AddCounts("Build", order.Structures.GetCounts());
                infoPanel.AddQuantities("Cost", order.GetTotalCost().GetQuantities());
            }
        }

        public void DescribeAll(IEnumerable<object> objects, InfoPanel infoPanel)
        {
            Describe(objects.First(), infoPanel);
        }

        public void Describe(object @object, InfoPanel infoPanel)
        {
            if (@object is BuildOrder)
            {
                new BuildOrderDescriber().Describe(@object, infoPanel);
            }
            else
            {
                throw new ArgumentException($"Unsupported order type: {@object.GetType()}");
            }
        }
    }
}
