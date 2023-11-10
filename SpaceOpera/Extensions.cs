namespace SpaceOpera
{
    public static class Extensions
    {
        public static TKey? ArgMaxRandomlySelecting<TKey, TValue>(
            this IEnumerable<TKey> Values, Func<TKey, TValue> ScoreFn, Random Random) where TValue : IComparable
        {
            TValue? maxScore = default;
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
                else if (score.CompareTo(maxScore) == 0 || Equals(maxScore, default))
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

        public static bool TryGetValueAs<TKey, TValue, TOut>(
            this IDictionary<TKey, TValue> dictionary, TKey key, out TOut? value) where TOut : TValue
        {
            if (dictionary.TryGetValue(key, out TValue? v))
            {
                value = (TOut)v!;
                return true;
            }

            value = default;
            return false;
        }
    }
}