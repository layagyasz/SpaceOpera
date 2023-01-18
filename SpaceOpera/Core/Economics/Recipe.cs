using Cardamom;
using Cardamom.Trackers;

namespace SpaceOpera.Core.Economics
{
    public class Recipe : IKeyed
    {
        private static readonly float s_Production = 1000f;

        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Structure? Structure { get; set; }
        public float Coefficient { get; set; }
        public MultiQuantity<IMaterial> Transformation { get; set; } = new();
        public IMaterial? BoundResourceNode { get; set; }

        public static Recipe ForDesignedMaterial(DesignedMaterial material, Structure structure)
        {
            var transformation = -1f * material.GetMaterialCost();
            transformation.Add(material, 1f);
            return new Recipe
            {
                Key = $"recipe-{material.Key}",
                Name = material.Name,
                Structure = structure,
                Coefficient = s_Production / material.ProductionCost,
                Transformation = transformation
            };
        }
    }
}
