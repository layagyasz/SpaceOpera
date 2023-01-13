using Cardamom.Collections;
using Cardamom.Trackers;

namespace SpaceOpera.Core.Languages
{
    public class PhonologySegment
    {
        private static readonly Phoneme s_Terminal = new();

        public double Entropy { get; }

        private readonly Dictionary<Phoneme, WeightedVector<Phoneme>> _allowedSequences;

        public PhonologySegment(Dictionary<Phoneme, WeightedVector<Phoneme>> allowedSequences)
        {
            _allowedSequences = allowedSequences;
            Entropy = GetEntropy();
        }

        public IEnumerable<Phoneme> GenerateSegment(Random random, bool requireNonVoid)
        {
            var current = s_Terminal;
            bool nonVoid = false;
            while (true)
            {
                var next = _allowedSequences[current].Get(random.NextSingle());
                if (next != s_Terminal)
                {
                    nonVoid = true;
                    yield return next;
                }
                else if (nonVoid || !requireNonVoid)
                {
                    yield break;
                }
                current = next;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "[PhonologySegment]\n{0}", 
                string.Join(
                    "\n", 
                    _allowedSequences.Select(
                        x => string.Format("{0} => {1}", x.Key, string.Join(",", x.Value.Select(y => y.Key))))));
        }

        private double GetEntropy()
        {
            return Math.Log(_allowedSequences.Sum(x => x.Value.Count)) / Math.Log(2);
        }

        public class Builder
        {
            private readonly MultiMap<Phoneme, Frequent<Phoneme>> _allowedSequences = new();

            public Builder AddPhonemes(
                IEnumerable<Frequent<Phoneme>> phonemes, float terminalFrequency, bool allowEmpty)
            {
                var p = phonemes.ToList();
                p.Add(new(s_Terminal, terminalFrequency));

                foreach (var phoneme in p)
                {
                    _allowedSequences.Add(phoneme.Value!, p);
                }
                if (!allowEmpty)
                {
                    _allowedSequences.RemoveAll(s_Terminal, x => x.Value == s_Terminal);
                }

                return this;
            }

            public Builder AddExclusion(PhonemeRange left, PhonemeRange right)
            {
                foreach (var entry in _allowedSequences)
                {
                    if (entry.Key != s_Terminal && left.Contains(entry.Key.Range))
                    {
                        _allowedSequences.RemoveAll(
                            entry.Key, x => x.Value != s_Terminal && right.Contains(x.Value!.Range));
                    }
                }

                return this;
            }

            public PhonologySegment Build()
            {
                var dict = new Dictionary<Phoneme, WeightedVector<Phoneme>>();
                foreach (var entry in _allowedSequences)
                {
                    var options = new WeightedVector<Phoneme>();
                    foreach (var value in entry.Value)
                    {
                        options.Add(value.Value, value.Frequency);
                    }
                    dict.Add(entry.Key, options);
                }
                return new PhonologySegment(dict);
            }
        }
    }
}