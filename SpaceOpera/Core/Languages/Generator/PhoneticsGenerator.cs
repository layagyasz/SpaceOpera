using Cardamom.Trackers;
using Cardamom.Utils.Generators.Samplers;

namespace SpaceOpera.Core.Languages.Generator
{
    public class PhoneticsGenerator
    {
        private static readonly ISampler s_FrequencyDropoff = new NormalSampler(0.8f, 0.9f);

        public List<Phoneme> Phonemes { get; set; } = new();
        public IndependentSelector<PhonemeRange>? Exclusions { get; set; }

        public Phonetics Generate(Random random)
        {
            var exlcusions = new List<PhonemeRange>();

            foreach (var newRange in Exclusions.Select(random))
            {
                exlcusions.Add(newRange);
            }

            var phonemes = new List<Phoneme>();
            foreach (var phoneme in Phonemes)
            {
                if(exlcusions.All(x => !x.Contains(phoneme.Range)))
                {
                    phonemes.Add(phoneme);
                }
            }

            var consonants = phonemes.Count(x => x.Range.Classes.Contains(PhonemeClass.Consonant));
            var consonantVowelRatio = 0.8f * consonants / (phonemes.Count - consonants);
            var phonemeRanks = new double[phonemes.Count];
            var phonemesArray = phonemes.ToArray();
            for (int i=0; i<phonemesArray.Length;++i)
            {
                phonemeRanks[i] = 
                    Math.Pow(
                        random.NextDouble(), 
                        phonemesArray[i].Range.Classes.Contains(PhonemeClass.Vowel) 
                            ? consonantVowelRatio : 1 / consonantVowelRatio);
            }
            Array.Sort(phonemeRanks, phonemesArray);

            double frequency = 1.0;
            var frequenices = new List<Frequent<Phoneme>>();
            foreach (var phoneme in phonemesArray)
            {
                frequenices.Add(new Frequent<Phoneme>(phoneme, (float)frequency));
                frequency *= Math.Min(1.0, s_FrequencyDropoff.Generate(random));
            }

            return new Phonetics(frequenices);
        }
    }
}