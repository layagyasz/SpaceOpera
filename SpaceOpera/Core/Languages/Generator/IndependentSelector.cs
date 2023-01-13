using Cardamom.Trackers;

namespace SpaceOpera.Core.Languages.Generator
{
    public class IndependentSelector<T>
    {
        public List<Frequent<DependentSelector<T>>> IndependentOptions { get; set; } = new();

        public IEnumerable<T> Select(Random random)
        {
            foreach (var option in IndependentOptions)
            {
                if (random.NextDouble() < option.Frequency)
                {
                    yield return option.Value!.Select(random);
                }
            }
        }
    }
}