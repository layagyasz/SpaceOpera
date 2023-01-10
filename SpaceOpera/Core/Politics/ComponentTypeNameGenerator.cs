using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static SpaceOpera.Core.Politics.ComponentNameGenerator;

namespace SpaceOpera.Core.Politics
{
    class ComponentTypeNameGenerator
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ComponentNameSource
        {
            NONE,

            STATIC,
            RANDOM,

            SEQUENCE_NUMBER,
            PARENT_NAME,
            TAGS,

            LANGUAGE_WORD,
            LANGUAGE_LETTER
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ComponentNameFilter
        {
            NONE,
            STRING,
            QUOTE_STRING,
            INTEGER,
            ORDINAL,
            ROMAN,
            TAG_SET
        }

        public class ComponentNamePart
        {
            public object StaticValue { get; }
            public RandomValue RandomValue { get; }
            public ComponentNameSource Source { get; }
            public ComponentNameFilter Filter { get; }

            public ComponentNamePart(
                object StaticValue, RandomValue RandomValue, ComponentNameSource Source, ComponentNameFilter Filter)
            {
                this.StaticValue = StaticValue;
                this.RandomValue = RandomValue;
                this.Source = Source;
                this.Filter = Filter;
            }
        }

        public List<ComponentNamePart> Pattern { get; }

        public ComponentTypeNameGenerator(IEnumerable<ComponentNamePart> Pattern)
        {
            this.Pattern = Pattern.ToList();
        }

        public string GenerateNameFor(
            NameGeneratorArgs Args, Language Language, List<ComponentTagName> ComponentTagNames, Random Random)
        {
            return string.Join(
                " ",
                Pattern.SelectMany(
                    x => NamePartToString(
                        x, Args, Language, ComponentTagNames, Random)).Where(x => x.Length > 0));
        }

        private IEnumerable<string> NamePartToString(
            ComponentNamePart Part,
            NameGeneratorArgs Args, 
            Language Language, 
            List<ComponentTagName> TagNames, 
            Random Random)
        {
            return ValueToString(NamePartToValue(Part, Args, Language, Random), Part.Filter, TagNames);
        }

        private object NamePartToValue(
            ComponentNamePart Part, NameGeneratorArgs Args, Language Language, Random Random)
        {
            switch (Part.Source)
            {
                case ComponentNameSource.STATIC:
                    return Part.StaticValue;
                case ComponentNameSource.RANDOM:
                    return Part.RandomValue.Generate(Random);
                case ComponentNameSource.SEQUENCE_NUMBER:
                    return Args.SequenceNumber;
                case ComponentNameSource.PARENT_NAME:
                    return Args.ParentName;
                case ComponentNameSource.TAGS:
                    return Args.Tags;
                case ComponentNameSource.LANGUAGE_WORD:
                    return Language.GenerateWord(Random);
                case ComponentNameSource.LANGUAGE_LETTER:
                    return Language.GenerateLetter(Random);
                default:
                    throw new ArgumentException(string.Format("Unsupported Source: [%s].", Part.Source));
            }
        }

        private IEnumerable<string> ValueToString(
            object Value, ComponentNameFilter Filter, List<ComponentTagName> TagNames)
        {
            switch (Filter)
            {
                case ComponentNameFilter.NONE:
                case ComponentNameFilter.STRING:
                case ComponentNameFilter.INTEGER:
                    return new List<string>() { StringUtils.FormatCase(Value.ToString()) };
                case ComponentNameFilter.QUOTE_STRING:
                    return new List<string>() { string.Format("\"{0}\"", StringUtils.FormatCase(Value.ToString())) };
                case ComponentNameFilter.ORDINAL:
                    return new List<string>() { ToOrdinal((long)Value) };
                case ComponentNameFilter.ROMAN:
                    return new List<string>() { ToRoman((long)Value) };
                case ComponentNameFilter.TAG_SET:
                    return TagsToString((List<ComponentTag>)Value, TagNames);
                default:
                    throw new ArgumentException(string.Format("Unsupported Filter: [%s].", Filter));
            }
        }

        private IEnumerable<string> TagsToString(IEnumerable<ComponentTag> Tags, List<ComponentTagName> TagNames)
        {
            var tagSet = new EnumSet<ComponentTag>(Tags);
            foreach (var tagName in TagNames)
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

        private static string ToOrdinal(long Value)
        {
            if (Value % 10 == 1 && Value % 100 != 11)
            {
                return Value.ToString() + "st";
            }
            if (Value % 10 == 2 && Value % 100 != 12)
            {
                return Value.ToString() + "nd";
            }
            if (Value % 10 == 3 && Value % 100 != 13)
            {
                return Value.ToString() + "rd";
            }
            return Value.ToString() + "th";
        }

        private static int[] ROMAN_VALUES = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
        private static String[] ROMAN_LITERALS = 
            { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
        private static string ToRoman(long Value)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < ROMAN_VALUES.Length; i++)
            {
                while (Value >= ROMAN_VALUES[i])
                {
                    Value -= ROMAN_VALUES[i];
                    result.Append(ROMAN_LITERALS[i]);
                }
            }
            return result.ToString();
        }
    }
}