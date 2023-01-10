using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera
{
    static class Extensions
    {
        public static TKey ArgMaxRandomlySelecting<TKey, TValue>(
            this IEnumerable<TKey> Values, Func<TKey, TValue> ScoreFn, Random Random) where TValue : IComparable
        {
            TValue maxScore = default;
            var options = new List<TKey>();
            foreach (var value in Values)
            {
                var score = ScoreFn(value);
                if (score.CompareTo(maxScore) == 1)
                {
                    maxScore = score;
                    options.Clear();
                    options.Add(value);
                }
                else if (score.CompareTo(maxScore) == 0 || maxScore == default)
                {
                    options.Add(value);
                }
            }
            if (options.Count == 0)
            {
                return default;
            }
            return options[Random.Next(0, options.Count)];
        }

        public static EnumMap<TKey, TValue> ToEnumMap<TKey, TValue, TIn>(
            this IEnumerable<TIn> Source, Func<TIn, TKey> KeySelector, Func<TIn, TValue> ValueSelector)
            where TKey : struct, IConvertible
        {
            var result = new EnumMap<TKey, TValue>();
            foreach (var value in Source)
            {
                result.Add(KeySelector(value), ValueSelector(value));
            }
            return result;
        }

        public static MultiCount<TKey> ToMultiCount<TKey, TIn>(
            this IEnumerable<TIn> Source, Func<TIn, TKey> KeySelector, Func<TIn, int> ValueSelector)
        {
            var result = new MultiCount<TKey>();
            foreach (var value in Source)
            {
                result.Add(KeySelector(value), ValueSelector(value));
            }
            return result;
        }

        public static MultiQuantity<TKey> ToMultiQuantity<TKey, TIn>(
            this IEnumerable<TIn> Source, Func<TIn, TKey> KeySelector, Func<TIn, float> ValueSelector)
        {
            var result = new MultiQuantity<TKey>();
            foreach (var value in Source)
            {
                result.Add(KeySelector(value), ValueSelector(value));
            }
            return result;
        }

        public static MultiCount<TKey> ToMultiCount<TKey>(this IEnumerable<TKey> Source)
        {
            var result = new MultiCount<TKey>();
            foreach (var value in Source.GroupBy(x => x))
            {
                result.Add(value.Key, value.Count());
            }
            return result;
        }
    }
}