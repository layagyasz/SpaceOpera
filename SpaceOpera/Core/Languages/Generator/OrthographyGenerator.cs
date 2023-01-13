using Cardamom.Trackers;

namespace SpaceOpera.Core.Languages.Generator
{
    public class OrthographyGenerator
    {
        private class Wrapper<T>
        {
            public T Value { get; }

            public Wrapper(T Value)
            {
                this.Value = Value;
            }
        }

        public List<OrthographySymbol> Symbols { get; set; } = new();
        public List<Frequent<OrthographySymbolModifier>> SymbolModifiers { get; set; } = new();

        public Orthography Generate(Phonetics Phonetics, Random Random)
        {
            var modifiers = 
                SymbolModifiers
                    .Where(x => Random.NextDouble() < x.Frequency)
                    .Select(x => x.Value)
                    .Select(x => x.Reduce(Random))
                    .ToList();
            var modifierWeights = modifiers.ToDictionary(x => x, x => Random.NextDouble());

            var symbols = new List<OrthographySymbol>();
            var symbolWeights = new Dictionary<OrthographySymbol, double>();
            foreach (var symbol in Symbols)
            {
                symbols.Add(symbol);
                symbolWeights.Add(symbol, Random.NextDouble());
                foreach (var modifier in modifiers)
                {
                    if (!modifier.IsApplicable(symbol))
                    {
                        continue;
                    }
                    var modified = modifier.Modify(symbol);
                    symbols.Add(modified);
                    symbolWeights.Add(modified, Random.NextDouble() * modifierWeights[modifier]);
                }
            }

            var symbolWrappers = new List<Wrapper<OrthographySymbol>>();
            while (symbolWrappers.Count < Phonetics.Phonemes.Count)
            {
                foreach (var symbol in symbols)
                {
                    symbolWrappers.Add(new Wrapper<OrthographySymbol>(symbol));
                }
            }

            var stableMatching = new StableMatching<Phoneme, Wrapper<OrthographySymbol>>();
            foreach (var symbol in symbolWrappers)
            {
                stableMatching.AddSecondaryActor(symbol);
            }
            foreach (var phoneme in Phonetics.Phonemes)
            {
                stableMatching.AddPrimaryActor(phoneme.Value);
                foreach (var symbol in symbolWrappers)
                {
                    stableMatching.SetPrimaryPreference(
                        phoneme.Value, 
                        symbol, 
                        -symbolWeights[symbol.Value] * symbol.Value.Range.Distance(phoneme.Value.Range));
                    stableMatching.SetSecondaryPreference(symbol, phoneme.Value, phoneme.Frequency);
                }
            }
            return new Orthography(
                stableMatching.GetPairs().Select(x => new OrthographyMatcher(x.First, x.Second.Value.Symbol)));
        }
    }
}