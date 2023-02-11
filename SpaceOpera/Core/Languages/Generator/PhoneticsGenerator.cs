using Cardamom.Trackers;
using Cardamom.Utils.Generators.Samplers;

namespace SpaceOpera.Core.Languages.Generator
{
    public class PhoneticsGenerator
    {
        private static readonly ISampler s_FrequencyDropoff = new UniformSampler(new(0.1f, 0.9f));

        public List<Phoneme> Phonemes { get; set; } = new();
        public IndependentSelector<PhonemeRange>? Exclusions { get; set; }

        public Phonetics Generate(GeneratorContext context)
        {
            var random = context.Random;
            var exlcusions = new List<PhonemeRange>();

            foreach (var newRange in Exclusions!.Select(random))
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
                        random.NextSingle(), 
                        phonemesArray[i].Range.Classes.Contains(PhonemeClass.Vowel) 
                            ? consonantVowelRatio : 1 / consonantVowelRatio);
            }
            Array.Sort(phonemeRanks, phonemesArray);

            float frequency = 1f;
            var frequencies = new List<Frequent<Phoneme>>();
            foreach (var phoneme in phonemesArray)
            {
                frequencies.Add(new Frequent<Phoneme>(phoneme, frequency));
                frequency *= Math.Min(1f, s_FrequencyDropoff.Generate(random));
            }

            return new Phonetics(frequencies);
        }
    }
}