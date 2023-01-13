using Cardamom.Collections;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Languages;
using System.Text;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Politics
{
    public class ComponentTypeNameGenerator
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ComponentNameSource
        {
            None,

            Static,
            Random,

            SequenceNumber,
            ParentName,
            Tags,

            LanguageWord,
            LanguageLetter
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ComponentNameFilter
        {
            None,
            String,
            QuoteString,
            Integer,
            Ordinal,
            Roman,
            TagSet
        }

        public class ComponentNamePart
        {
            public object? StaticValue { get; }
            public RandomValue? RandomValue { get; }
            public ComponentNameSource Source { get; }
            public ComponentNameFilter Filter { get; }

            public ComponentNamePart(
                object? staticValue, RandomValue? randomValue, ComponentNameSource source, ComponentNameFilter filter)
            {
                StaticValue = staticValue;
                RandomValue = randomValue;
                Source = source;
                Filter = filter;
            }
        }

        public List<ComponentNamePart> Pattern { get; }

        public ComponentTypeNameGenerator(IEnumerable<ComponentNamePart> pattern)
        {
            Pattern = pattern.ToList();
        }

        public string GenerateNameFor(
            NameGeneratorArgs args, Language language, List<ComponentTagName> componentTagNames, Random random)
        {
            return string.Join(
                " ",
                Pattern.SelectMany(
                    x => NamePartToString(
                        x, args, language, componentTagNames, random)).Where(x => x.Length > 0));
        }

        private static IEnumerable<string> NamePartToString(
            ComponentNamePart part,
            NameGeneratorArgs args, 
            Language language, 
            List<ComponentTagName> tagNames, 
            Random random)
        {
            return ValueToString(NamePartToValue(part, args, language, random), part.Filter, tagNames);
        }

        private static object NamePartToValue(
            ComponentNamePart part, NameGeneratorArgs args, Language language, Random random)
        {
            switch (part.Source)
            {
                case ComponentNameSource.Static:
                    return part!.StaticValue;
                case ComponentNameSource.Random:
                    return part!.RandomValue.Generate(random);
                case ComponentNameSource.SequenceNumber:
                    return args.SequenceNumber;
                case ComponentNameSource.ParentName:
                    return args.ParentName;
                case ComponentNameSource.Tags:
                    return args.Tags;
                case ComponentNameSource.LanguageWord:
                    return language.GenerateWord(random);
                case ComponentNameSource.LanguageLetter:
                    return language.GenerateLetter(random);
                case ComponentNameSource.None:
                default:
                    throw new ArgumentException($"Unsupported Source: [{part.Source}].");
            }
        }

        private static IEnumerable<string> ValueToString(
            object value, ComponentNameFilter filter, List<ComponentTagName> tagNames)
        {
            switch (filter)
            {
                case ComponentNameFilter.None:
                case ComponentNameFilter.String:
                case ComponentNameFilter.Integer:
                    return new List<string>() { StringUtils.FormatCase(value.ToString()!) };
                case ComponentNameFilter.QuoteString:
                    return new List<string>() { string.Format("\"{0}\"", StringUtils.FormatCase(value.ToString()!)) };
                case ComponentNameFilter.Ordinal:
                    return new List<string>() { ToOrdinal((long)value) };
                case ComponentNameFilter.Roman:
                    return new List<string>() { ToRoman((long)value) };
                case ComponentNameFilter.TagSet:
                    return TagsToString((List<ComponentTag>)value, tagNames);
                default:
                    throw new ArgumentException($"Unsupported Filter: [{filter}].");
            }
        }

        private static IEnumerable<string> TagsToString(
            IEnumerable<ComponentTag> tags, List<ComponentTagName> tagNames)
        {
            var tagSet = new EnumSet<ComponentTag>(tags);
            foreach (var tagName in tagNames)
            {
                if (tagSet.IsSupersetOf(tagName.Tags))
                {
                    tagSet.ExceptWith(tagName.Tags);
                    yield return StringUtils.FormatCase(tagName.Name);
                    if (tagSet.Count == 0)
                    {
                        yield break;
                    }
                }
            }
        }

        private static string ToOrdinal(long value)
        {
            if (value % 10 == 1 && value % 100 != 11)
            {
                return value.ToString() + "st";
            }
            if (value % 10 == 2 && value % 100 != 12)
            {
                return value.ToString() + "nd";
            }
            if (value % 10 == 3 && value % 100 != 13)
            {
                return value.ToString() + "rd";
            }
            return value.ToString() + "th";
        }

        private static readonly int[] s_RomanValues = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
        private static readonly string[] s_RomanLiterals = 
            { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
        private static string ToRoman(long value)
        {
            StringBuilder result = new();
            for (int i = 0; i < s_RomanValues.Length; i++)
            {
                while (value >= s_RomanValues[i])
                {
                    value -= s_RomanValues[i];
                    result.Append(s_RomanLiterals[i]);
                }
            }
            return result.ToString();
        }
    }
}