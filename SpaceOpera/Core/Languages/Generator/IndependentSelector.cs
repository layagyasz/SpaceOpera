using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages.Generator
{
    class IndependentSelector<T>
    {
        public List<Frequent<DependentSelector<T>>> IndependentOptions { get; set; }

        public IEnumerable<T> Select(Random Random)
        {
            foreach (var option in IndependentOptions)
            {
                if (Random.NextDouble() < option.Frequency)
                {
                    yield return option.Value.Select(Random);
                }
            }
        }
    }
}