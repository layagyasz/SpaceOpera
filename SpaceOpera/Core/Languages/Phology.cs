using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages
{
    class Phonology
    {
        public double Entropy { get; }

        private readonly PhonologySegment _Onset;
        private readonly PhonologySegment _Nucleus;
        private readonly PhonologySegment _Offset;

        public Phonology(PhonologySegment Onset, PhonologySegment Nucleus, PhonologySegment Offset)
        {
            _Onset = Onset;
            _Nucleus = Nucleus;
            _Offset = Offset;

            Entropy = GetEntropy();
        }

        public List<Phoneme> GenerateSyllable(Random Random, bool RequireOnset, out bool VoidOffset)
        {
            var onset = _Onset.GenerateSegment(Random, RequireOnset).ToList();
            var nucleus = _Nucleus.GenerateSegment(Random, false).ToList();
            var offset = _Offset.GenerateSegment(Random, false).ToList();

            VoidOffset = offset.Count == 0;

            onset.AddRange(nucleus);
            onset.AddRange(offset);
            return onset;
        }

        public override string ToString()
        {
            return string.Format("[Phology]\n[Onset]\n{0}\n[Nucleus]\n{1}\n[Offset]\n{2}", _Onset, _Nucleus, _Offset);
        }

        private double GetEntropy()
        {
            return _Onset.Entropy + _Nucleus.Entropy + _Offset.Entropy;
        }

        public class Builder
        {
            private readonly PhonologySegment.Builder _Onset;
            private readonly PhonologySegment.Builder _Nucleus;
            private readonly PhonologySegment.Builder _Offset;

            public Builder(IEnumerable<Frequent<Phoneme>> Phonemes)
            {
                var consonants = Phonemes.Where(x => x.Value.Range.Classes.Contains(PhonemeClass.CONSONANT)).ToList();
                var vowels = Phonemes.Where(x => x.Value.Range.Classes.Contains(PhonemeClass.VOWEL)).ToList();
                var consonantWeight = consonants.Sum(x => x.Frequency);
                var vowelWeight = vowels.Sum(x => x.Frequency);

                _Onset = new PhonologySegment.Builder().AddPhonemes(consonants, vowelWeight, true);
                _Nucleus = new PhonologySegment.Builder().AddPhonemes(vowels, consonantWeight, false);
                _Offset = new PhonologySegment.Builder().AddPhonemes(consonants, vowelWeight, true);
            }

            public void AddOnsetExclusion(PhonemeRange Left, PhonemeRange Right)
            {
                _Onset.AddExclusion(Left, Right);
            }

            public void AddNucleusExclusion(PhonemeRange Left, PhonemeRange Right)
            {
                _Nucleus.AddExclusion(Left, Right);
            }

            public void AddOffsetExclusion(PhonemeRange Left, PhonemeRange Right)
            {
                _Offset.AddExclusion(Left, Right);
            }

            public Phonology Build()
            {
                return new Phonology(_Onset.Build(), _Nucleus.Build(), _Offset.Build());
            }
        }
    }
}