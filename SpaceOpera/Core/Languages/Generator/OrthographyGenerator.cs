using Cardamom.Graphing;
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
                    .Select(x => x.Value!.Reduce(Random))
                    .ToList();
            var modifierWeights = modifiers.ToDictionary(x => x, x => Random.NextSingle());

            var symbols = new List<OrthographySymbol>();
            var symbolWeights = new Dictionary<OrthographySymbol, float>();
            foreach (var symbol in Symbols)
            {
                symbols.Add(symbol);
                symbolWeights.Add(symbol, Random.NextSingle());
                foreach (var modifier in modifiers)
                {
                    if (!modifier.IsApplicable(symbol))
                    {
                        continue;
                    }
                    var modified = modifier.Modify(symbol);
                    symbols.Add(modified);
                    symbolWeights.Add(modified, Random.NextSingle() * modifierWeights[modifier]);
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

            return new Orthography(
                StableMatching.Compute(
                    Phonetics.Phonemes,
                    symbolWrappers,
                    (phoneme, symbol) => 
                        -symbolWeights[symbol.Value] * symbol.Value.Range.Distance(phoneme.Value!.Range),
                    (phoneme, _) => 
                        phoneme.Frequency).Select(x => new OrthographyMatcher(x.Item1.Value!, x.Item2.Value.Symbol)));
        }
    }
}