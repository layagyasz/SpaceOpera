﻿using Cardamom;
using Cardamom.Trackers;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Designs;

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
        public HashSet<IAdvancement> Prerequisites { get; set; } = new();

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
                Transformation = transformation,
                Prerequisites = 
                    transformation.Keys
                        .SelectMany(x => x is IComponent c ? c.Prerequisites : Enumerable.Empty<IAdvancement>())
                        .ToHashSet()
            };
        }
    }
}
