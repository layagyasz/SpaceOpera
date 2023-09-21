using Cardamom.Graphing;
using Cardamom.Trackers;

namespace SpaceOpera.Core.Languages.Generator
{
    public class OrthographyGenerator
    {
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
            var modifierWeights = modifiers.ToDictionary(x => x, x => 1 / random.NextSingle());

            var symbols = new List<OrthographySymbol>();
            var symbolWeights = new Dictionary<OrthographySymbol, float>();
            foreach (var symbol in Symbols)
            {
                symbols.Add(symbol);
                var baseWeight = random.NextSingle();
                symbolWeights.Add(symbol, baseWeight);
                foreach (var modifier in modifiers)
                {
                    if (!modifier.IsApplicable(symbol))
                    {
                        continue;
                    }
                    var modified = modifier.Modify(symbol);
                    symbols.Add(modified);
                    symbolWeights.Add(modified, baseWeight * modifierWeights[modifier]);
                }
            }

            var symbolWrappers = new List<Frequent<OrthographySymbol>>();

            var vowelsSymbols = symbols.Where(x => x.Range.Classes.Contains(PhonemeClass.Vowel)).ToList();
            for (
                int i = 0, m = 1; 
                i < phonetics.Phonemes.Where(x => x.Value!.Range.Classes.Contains(PhonemeClass.Vowel)).Count() 
                    / vowelsSymbols.Count + 1;
                ++i, m *= 2)
            {
                foreach (var symbol in vowelsSymbols)
                {
                    symbolWrappers.Add(new(symbol, m * symbolWeights[symbol]));
                }
            }

            var consonantSymbols = symbols.Where(x => x.Range.Classes.Contains(PhonemeClass.Consonant)).ToList();
            for (
                int i = 0, m = 1;
                i < phonetics.Phonemes.Where(x => x.Value!.Range.Classes.Contains(PhonemeClass.Consonant)).Count()
                    / consonantSymbols.Count + 1;
                ++i, m *= 2)
            {
                foreach (var symbol in consonantSymbols)
                {
                    symbolWrappers.Add(new(symbol, m * symbolWeights[symbol]));
                }
            }

            return new Orthography(
                StableMatching.Compute(
                    phonetics.Phonemes,
                    symbolWrappers,
                    (phoneme, symbol) => symbol.Frequency * symbol.Value!.Range.Distance(phoneme.Value!.Range),
                    (phoneme, _) => -phoneme.Frequency)
                .Select(x => new OrthographyMatcher(x.Item1.Value!, x.Item2.Value!.Symbol)));
        }
    }
}