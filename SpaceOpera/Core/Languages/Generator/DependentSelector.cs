using Cardamom.Collections;
using Cardamom.Json.Collections;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Languages.Generator
{
    public class DependentSelector<T>
    {
        [JsonConverter(typeof(ListDictionaryJsonConverter))]
        public WeightedVector<T> DependentOptions { get; set; } = new();

        public T Select(Random random)
        {
            return DependentOptions.Get(random.NextSingle());
        }
    }
}