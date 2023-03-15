using Cardamom.Trackers;
using SpaceOpera.Core.Economics;

namespace SpaceOpera.View.Info
{
    public class RecipeDescriber : IDescriber
    {
        public void Describe(IEnumerable<object> objects, InfoPanel infoPanel)
        {
            throw new NotImplementedException();
        }

        public void Describe(object @object, InfoPanel infoPanel)
        {
            var recipe = (Recipe)@object;
            infoPanel.AddTitle(@object, recipe.Name);
            infoPanel.AddQuantities(
                "Input",
                recipe.Transformation.GetQuantities().Where(
                    x => x.Value < 0).Select(x => Quantity<IMaterial>.Create(x.Key, -x.Value)));
            infoPanel.AddQuantities("Output", recipe.Transformation.GetQuantities().Where(x => x.Value > 0));
        }
    }
}