using SpaceOpera.Core.Economics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core
{
    class GameModifier : IKeyed
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public List<SingleGameModifier> Modifiers { get; set; }

        public static Modifier AggregatePopulationGeneration(IEnumerable<GameModifier> Modifiers)
        {
            return Aggregate(Modifiers, ModifierType.POPULATION_GENERATION);
        }

        public static Dictionary<IMaterial, Modifier> AggregateResourceGeneration(IEnumerable<GameModifier> Modifiers)
        {
            return Aggregate<IMaterial, Modifier>(Modifiers, ModifierType.RESOURCE_GENERATION, x => x.Material);
        }

        private static Modifier Aggregate(IEnumerable<GameModifier> Modifiers, ModifierType Type)
        {
            if (Modifiers != null)
            {
                return Modifiers
                    .SelectMany(x => x.Modifiers)
                    .Where(x => x.Type == Type)
                    .Select(x => x.Modifier)
                    .Aggregate(Modifier.Zero, (x, y) => x + y);
            }
            return Modifier.Zero;
        }

        private static Dictionary<TKey, Modifier> Aggregate<TKey, TValue>(
            IEnumerable<GameModifier> Modifiers, ModifierType Type, Func<SingleGameModifier, TKey> KeyFn)
        {
            var result = new Dictionary<TKey, Modifier>();
            if (Modifiers != null)
            {
                foreach (var modifier in Modifiers.SelectMany(x => x.Modifiers).Where(x => x.Type == Type))
                {
                    var key = KeyFn(modifier);
                    if (result.ContainsKey(key))
                    {
                        result[key] += modifier.Modifier;
                    }
                    else
                    {
                        result[key] = modifier.Modifier;
                    }
                }
            }
            return result;
        }
    }
}