using Cardamom.Trackers;

namespace SpaceOpera.Core.Economics
{
    public class ActualizedRecipe
    {
        public int SubKey { get; }
        public Recipe BaseRecipe { get; }
        public MultiQuantity<IMaterial> Transformation { get; }

        public ActualizedRecipe(int subKey, Recipe baseRecipe, MultiQuantity<IMaterial> transformation)
        {
            SubKey = subKey;
            BaseRecipe = baseRecipe;
            Transformation = transformation;
        }

        public override int GetHashCode()
        {
            if (BaseRecipe.BoundResourceNode == null)
            {
                return BaseRecipe.GetHashCode();
            }
            return SubKey ^ BaseRecipe.GetHashCode();
        }
    }
}