using Cardamom.Collections;

namespace SpaceOpera.Core.Languages.Generator
{
    public class OrthographySymbolModifier
    {
        public PhonemeRange Range { get; set; } = PhonemeRange.CreateFull();
        public PhonemeRange ModifiedRange { get; set; } = PhonemeRange.CreateEmpty();
        public Dictionary<string, string>? SymbolMap { get; set; }
        public string Modifier { get; set; } = string.Empty;

        public bool IsApplicable(OrthographySymbol symbol)
        {
            return Range.Contains(symbol.Range) && (SymbolMap == null || SymbolMap.ContainsKey(symbol.Symbol));
        }

        public OrthographySymbol Modify(OrthographySymbol symbol)
        {
            return new OrthographySymbol()
            {
                Symbol = SymbolMap == null ? symbol.Symbol + Modifier : SymbolMap[symbol.Symbol],
                Range = symbol.Range.Union(ModifiedRange)
            };
        }

        public OrthographySymbolModifier Reduce(Random random)
        {
            var reducers = new WeightedVector<Func<PhonemeRange, Random, PhonemeRange>>
            {
                { (x, y) => PhonemeRange.CreateEmpty(), 1 },
                { ReduceClasses, ModifiedRange.Classes.Count },
                { ReduceVoices, ModifiedRange.Voices.Count },
                { ReduceTypes, ModifiedRange.Types.Count },
                { ReducePositions, ModifiedRange.Positions.Count }
            };
            return new OrthographySymbolModifier()
            {
                Range = Range,
                ModifiedRange = reducers.Get(random.NextSingle())(ModifiedRange, random),
                SymbolMap = SymbolMap,
                Modifier = Modifier
            };
        }

        private static PhonemeRange ReduceClasses(PhonemeRange range, Random random)
        {
            return new PhonemeRange()
            {
                Classes = new EnumSet<PhonemeClass>(range.Classes.ToArray()[random.Next(0, range.Classes.Count)]),
            };
        }

        private static PhonemeRange ReduceVoices(PhonemeRange range, Random random)
        {
            return new PhonemeRange()
            {
                Voices = SelectRandom(range.Voices, random),
            };
        }

        private static PhonemeRange ReduceTypes(PhonemeRange range, Random random)
        {
            return new PhonemeRange()
            {
                Types = SelectRandom(range.Types, random),
            };
        }

        private static PhonemeRange ReducePositions(PhonemeRange range, Random random)
        {
            return new PhonemeRange()
            {
                Positions = SelectRandom(range.Positions, random)
            };
        }

        private static EnumSet<T> SelectRandom<T>(EnumSet<T> set, Random random) where T : Enum
        {
            return new EnumSet<T>(set.ToArray()[random.Next(0, set.Count)]);
        }
    }
}