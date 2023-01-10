using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages
{
    class PhonologySegment
    {
        private static readonly Phoneme TERMINAL = new Phoneme();

        public double Entropy { get; }

        private readonly Dictionary<Phoneme, WeightedVector<Phoneme>> _AllowedSequences;

        public PhonologySegment(Dictionary<Phoneme, WeightedVector<Phoneme>> AllowedSequences)
        {
            _AllowedSequences = AllowedSequences;
            Entropy = GetEntropy();
        }

        public IEnumerable<Phoneme> GenerateSegment(Random Random, bool RequireNonVoid)
        {
            var current = TERMINAL;
            bool nonVoid = false;
            while (true)
            {
                var next = _AllowedSequences[current][Random.NextDouble()];
                if (next != TERMINAL)
                {
                    nonVoid = true;
                    yield return next;
                }
                else if (nonVoid || !RequireNonVoid)
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
                    _AllowedSequences.Select(
                        x => string.Format("{0} => {1}", x.Key, string.Join(",", x.Value.Select(y => y.Key))))));
        }

        private double GetEntropy()
        {
            return Math.Log(_AllowedSequences.Sum(x => x.Value.Count)) / Math.Log(2);
        }

        public class Builder
        {
            private readonly MultiMap<Phoneme, Frequent<Phoneme>> _AllowedSequences =
                new MultiMap<Phoneme, Frequent<Phoneme>>();

            public Builder AddPhonemes(
                IEnumerable<Frequent<Phoneme>> Phonemes, float TerminalFrequency, bool AllowEmpty)
            {
                var phonemes = Phonemes.ToList();
                phonemes.Add(new Frequent<Phoneme>(TERMINAL, TerminalFrequency));

                foreach (var phoneme in phonemes)
                {
                    _AllowedSequences.Add(phoneme.Value, phonemes);
                }
                if (!AllowEmpty)
                {
                    _AllowedSequences.RemoveAll(TERMINAL, x => x.Value == TERMINAL);
                }

                return this;
            }

            public Builder AddExclusion(PhonemeRange Left, PhonemeRange Right)
            {
                foreach (var entry in _AllowedSequences)
                {
                    if (entry.Key != TERMINAL && Left.Contains(entry.Key.Range))
                    {
                        _AllowedSequences.RemoveAll(
                            entry.Key, x => x.Value != TERMINAL && Right.Contains(x.Value.Range));
                    }
                }

                return this;
            }

            public PhonologySegment Build()
            {
                var dict = new Dictionary<Phoneme, WeightedVector<Phoneme>>();
                foreach (var entry in _AllowedSequences)
                {
                    var options = new WeightedVector<Phoneme>();
                    foreach (var value in entry.Value)
                    {
                        options.Add(value.Frequency, value.Value);
                    }
                    dict.Add(entry.Key, options);
                }
                return new PhonologySegment(dict);
            }
        }
    }
}