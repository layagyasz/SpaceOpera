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

        public Orthography Generate(Phonetics phonetics, GeneratorContext context)
        {
            var random = context.Random;
            var modifiers = 
                SymbolModifiers
                    .Where(x => random.NextSingle() < x.Frequency)
                    .Select(x => x.Value!.Reduce(random))
                    .ToList();
            var modifierWeights = modifiers.ToDictionary(x => x, x => random.NextSingle());

            var symbols = new List<OrthographySymbol>();
            var symbolWeights = new Dictionary<OrthographySymbol, float>();
            foreach (var symbol in Symbols)
            {
                symbols.Add(symbol);
                symbolWeights.Add(symbol, random.NextSingle());
                foreach (var modifier in modifiers)
                {
                    if (!modifier.IsApplicable(symbol))
                    {
                        continue;
                    }
                    var modified = modifier.Modify(symbol);
                    symbols.Add(modified);
                    symbolWeights.Add(modified, random.NextSingle() * modifierWeights[modifier]);
                }
            }

            var symbolWrappers = new List<Wrapper<OrthographySymbol>>();
            while (symbolWrappers.Count < phonetics.Phonemes.Count)
            {
                foreach (var symbol in symbols)
                {
                    symbolWrappers.Add(new Wrapper<OrthographySymbol>(symbol));
                }
            }

            return new Orthography(
                StableMatching.Compute(
                    phonetics.Phonemes,
                    symbolWrappers,
                    (phoneme, symbol) => 
                        -symbolWeights[symbol.Value] * symbol.Value.Range.Distance(phoneme.Value!.Range),
                    (phoneme, _) => 
                        phoneme.Frequency).Select(x => new OrthographyMatcher(x.Item1.Value!, x.Item2.Value.Symbol)));
        }
    }
}