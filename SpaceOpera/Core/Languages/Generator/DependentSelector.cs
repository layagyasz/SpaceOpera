using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages.Generator
{
    class DependentSelector<T>
    {
        public List<Frequent<T>> DependentOptions { get; set; }

        public T Select(Random Random)
        {
            WeightedVector<T> options = new WeightedVector<T>();
            foreach (var option in DependentOptions)
            {
                options.Add(option.Frequency, option.Value);
            }
            return options[Random.NextDouble()];
        }
    }
}