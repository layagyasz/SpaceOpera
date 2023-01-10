using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages.Generator
{
    class PhoneticsGenerator
    {
        private static readonly Sampler FREQUENCY_DROPOFF = new Sampler(Sampler.SamplerType.NORMAL, 0.8, 0.1);

        public List<Phoneme> Phonemes { get; set; }
        public IndependentSelector<PhonemeRange> Exclusions { get; set; }

        public Phonetics Generate(Random Random)
        {
            var exlcusions = new List<PhonemeRange>();

            foreach (var newRange in Exclusions.Select(Random))
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

            var consonants = phonemes.Count(x => x.Range.Classes.Contains(PhonemeClass.CONSONANT));
            var consonantVowelRatio = 0.8f * consonants / (phonemes.Count - consonants);
            var phonemeRanks = new double[phonemes.Count];
            var phonemesArray = phonemes.ToArray();
            for (int i=0; i<phonemesArray.Length;++i)
            {
                phonemeRanks[i] = 
                    Math.Pow(
                        Random.NextDouble(), 
                        phonemesArray[i].Range.Classes.Contains(PhonemeClass.VOWEL) 
                            ? consonantVowelRatio : 1 / consonantVowelRatio);
            }
            Array.Sort(phonemeRanks, phonemesArray);

            double frequency = 1.0;
            var frequenices = new List<Frequent<Phoneme>>();
            foreach (var phoneme in phonemesArray)
            {
                frequenices.Add(new Frequent<Phoneme>(phoneme, (float)frequency));
                frequency *= Math.Min(1.0, FREQUENCY_DROPOFF.Sample(Random));
            }

            return new Phonetics(frequenices);
        }
    }
}