using SpaceOpera.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages.Generator
{
    class OrthographySymbolModifier
    { 
        public PhonemeRange Range { get; set; }
        public PhonemeRange ModifiedRange { get; set; }

        [JsonConverter(typeof(DictionaryJsonConverter<string, string>))]
        public Dictionary<string, string> SymbolMap { get; set; }
        public string Modifier { get; set; }

        public bool IsApplicable(OrthographySymbol Symbol)
        {
            return (Range == null || Range.Contains(Symbol.Range))
                && (SymbolMap == null || SymbolMap.ContainsKey(Symbol.Symbol));
        }

        public OrthographySymbol Modify(OrthographySymbol Symbol)
        {
            return new OrthographySymbol()
            {
                Symbol = SymbolMap == null ? Symbol.Symbol + Modifier : SymbolMap[Symbol.Symbol],
                Range = Symbol.Range.Union(ModifiedRange)
            };
        }

        public OrthographySymbolModifier Reduce(Random Random)
        {
            var reducers = new WeightedVector<Func<PhonemeRange, Random, PhonemeRange>>
            {
                { 1, (x, y) => PhonemeRange.CreateEmpty() },
                { ModifiedRange.Classes.Count, ReduceClasses },
                { ModifiedRange.Voices.Count, ReduceVoices },
                { ModifiedRange.Types.Count, ReduceTypes },
                { ModifiedRange.Positions.Count, ReducePositions }
            };
            return new OrthographySymbolModifier()
            {
                Range = Range,
                ModifiedRange = reducers[Random.NextDouble()](ModifiedRange, Random),
                SymbolMap = SymbolMap,
                Modifier = Modifier
            };
        }

        private static PhonemeRange ReduceClasses(PhonemeRange Range, Random Random)
        {
            return new PhonemeRange()
            {
                Classes = new EnumSet<PhonemeClass>(Range.Classes.ToArray()[Random.Next(0, Range.Classes.Count)]),
                Voices = new EnumSet<PhonemeVoice>(),
                Types = new EnumSet<PhonemeType>(),
                Positions = new EnumSet<PhonemePosition>()
            };
        }

        private static PhonemeRange ReduceVoices(PhonemeRange Range, Random Random)
        {
            return new PhonemeRange()
            {
                Classes = new EnumSet<PhonemeClass>(),
                Voices = SelectRandom(Range.Voices, Random),
                Types = new EnumSet<PhonemeType>(),
                Positions = new EnumSet<PhonemePosition>()
            };
        }

        private static PhonemeRange ReduceTypes(PhonemeRange Range, Random Random)
        {
            return new PhonemeRange()
            {
                Classes = new EnumSet<PhonemeClass>(),
                Voices = new EnumSet<PhonemeVoice>(),
                Types = SelectRandom(Range.Types, Random),
                Positions = new EnumSet<PhonemePosition>()
            };
        }

        private static PhonemeRange ReducePositions(PhonemeRange Range, Random Random)
        {
            return new PhonemeRange()
            {
                Classes = new EnumSet<PhonemeClass>(),
                Voices = new EnumSet<PhonemeVoice>(),
                Types = new EnumSet<PhonemeType>(),
                Positions = SelectRandom(Range.Positions, Random)
            };
        }

        private static EnumSet<T> SelectRandom<T>(EnumSet<T> Set, Random Random)
        {
            return new EnumSet<T>(Set.ToArray()[Random.Next(0, Set.Count)]);
        }
    }
}