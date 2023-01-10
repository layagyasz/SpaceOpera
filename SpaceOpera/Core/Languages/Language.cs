using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages
{
    class Language
    {
        public Phonetics Phonetics { get; }
        public Orthography Orthography { get; }
        public Phonology Phonology { get; }
        public Sampler WordLengthSampler { get; }

        public Language(Phonetics Phonetics, Orthography Orthography, Phonology Phonology)
        {
            this.Phonetics = Phonetics;
            this.Orthography = Orthography;
            this.Phonology = Phonology;
            this.WordLengthSampler =
                new Sampler(Sampler.SamplerType.NORMAL, 32 / Phonology.Entropy, 8 / Phonology.Entropy);
        }

        public string GenerateLetter(Random Random)
        {
            return Orthography.Transcribe(
                Phonology.GenerateSyllable(Random, false, out bool voidOffset), Random).First();
        }

        public string GenerateWord(Random Random)
        {
            var phonemes = new List<Phoneme>();
            bool requireOnset = false;
            for (int i=0; i<Math.Max(1, Math.Round(WordLengthSampler.Sample(Random))); ++i)
            {
                phonemes.AddRange(Phonology.GenerateSyllable(Random, requireOnset, out bool voidOffset));
            }
            return string.Join("", Orthography.Transcribe(phonemes, Random));
        }

        public override string ToString()
        {
            return string.Format("[Language]\n{0}\n{1}\n{2}", Phonetics, Orthography, Phonology);
        }
    }
}