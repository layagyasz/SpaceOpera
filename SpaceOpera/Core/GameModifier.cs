using Cardamom;
using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core
{
    public class GameModifier : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<SingleGameModifier> Modifiers { get; set; } = new();

        public static GameModifier Create(string key, string name, SingleGameModifier modifier)
        {
            return new GameModifier()
            {
                Key = key,
                Name = name,
                Modifiers = new List<SingleGameModifier>() { modifier }
            };
        }

        public static Modifier AggregatePopulationGeneration(IEnumerable<GameModifier> modifiers)
        {
            return Aggregate(modifiers, ModifierType.PopulationGeneration);
        }

        public static Dictionary<IMaterial, Modifier> AggregateResourceGeneration(IEnumerable<GameModifier> modifiers)
        {
            return Aggregate<IMaterial, Modifier>(modifiers, ModifierType.ResourceGeneration, x => x.Material!);
        }

        private static Modifier Aggregate(IEnumerable<GameModifier> modifiers, ModifierType type)
        {
            if (modifiers != null)
            {
                return modifiers
                    .SelectMany(x => x.Modifiers)
                    .Where(x => x.Type == type)
                    .Select(x => x.Modifier)
                    .Aggregate(Modifier.Zero, (x, y) => x + y);
            }
            return Modifier.Zero;
        }

        private static Dictionary<TKey, Modifier> Aggregate<TKey, TValue>(
            IEnumerable<GameModifier> modifiers, ModifierType type, Func<SingleGameModifier, TKey> keyFn) 
            where TKey: notnull
        {
            var result = new Dictionary<TKey, Modifier>();
            if (modifiers != null)
            {
                foreach (var modifier in modifiers.SelectMany(x => x.Modifiers).Where(x => x.Type == type))
                {
                    var key = keyFn(modifier);
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