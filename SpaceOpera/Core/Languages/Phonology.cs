using Cardamom.Trackers;

namespace SpaceOpera.Core.Languages
{
    public class Phonology
    {
        public float Entropy { get; }

        private readonly PhonologySegment _onset;
        private readonly PhonologySegment _nucleus;
        private readonly PhonologySegment _offset;

        public Phonology(PhonologySegment onset, PhonologySegment nucleus, PhonologySegment offset)
        {
            _onset = onset;
            _nucleus = nucleus;
            _offset = offset;

            Entropy = GetEntropy();
        }

        public List<Phoneme> GenerateSyllable(Random random, bool requireOnset, out bool voidOffset)
        {
            var onset = _onset.GenerateSegment(random, requireOnset).ToList();
            var nucleus = _nucleus.GenerateSegment(random, false).ToList();
            var offset = _offset.GenerateSegment(random, false).ToList();

            voidOffset = offset.Count == 0;

            onset.AddRange(nucleus);
            onset.AddRange(offset);
            return onset;
        }

        public override string ToString()
        {
            return string.Format(
                "[Phonology]\n[Onset]\n{0}\n[Nucleus]\n{1}\n[Offset]\n{2}", _onset, _nucleus, _offset);
        }

        private float GetEntropy()
        {
            return _onset.Entropy + _nucleus.Entropy + _offset.Entropy;
        }

        public class Builder
        {
            private readonly PhonologySegment.Builder _onset;
            private readonly PhonologySegment.Builder _nucleus;
            private readonly PhonologySegment.Builder _offset;

            public Builder(IEnumerable<Frequent<Phoneme>> phonemes)
            {
                var consonants = phonemes.Where(x => x.Value!.Range.Classes.Contains(PhonemeClass.Consonant)).ToList();
                var vowels = phonemes.Where(x => x.Value!.Range.Classes.Contains(PhonemeClass.Vowel)).ToList();
                var consonantWeight = consonants.Sum(x => x.Frequency);
                var vowelWeight = vowels.Sum(x => x.Frequency);

                _onset = new PhonologySegment.Builder().AddPhonemes(consonants, vowelWeight, true);
                _nucleus = new PhonologySegment.Builder().AddPhonemes(vowels, consonantWeight, false);
                _offset = new PhonologySegment.Builder().AddPhonemes(consonants, vowelWeight, true);
            }

            public void AddOnsetExclusion(PhonemeRange Left, PhonemeRange Right)
            {
                _onset.AddExclusion(Left, Right);
            }

            public void AddNucleusExclusion(PhonemeRange Left, PhonemeRange Right)
            {
                _nucleus.AddExclusion(Left, Right);
            }

            public void AddOffsetExclusion(PhonemeRange Left, PhonemeRange Right)
            {
                _offset.AddExclusion(Left, Right);
            }

            public Phonology Build()
            {
                return new Phonology(_onset.Build(), _nucleus.Build(), _offset.Build());
            }
        }
    }
}