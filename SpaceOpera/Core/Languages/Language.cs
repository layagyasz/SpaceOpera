using Cardamom.Utils.Generators.Samplers;

namespace SpaceOpera.Core.Languages
{
    public class Language
    {
        public Phonetics? Phonetics { get; }
        public Orthography? Orthography { get; }
        public Phonology? Phonology { get; }
        public ISampler WordLengthSampler { get; }

        public Language(Phonetics phonetics, Orthography orthography, Phonology phonology)
        {
            Phonetics = phonetics;
            Orthography = orthography;
            Phonology = phonology;
            WordLengthSampler = new UniformSampler(new(32 / phonology.Entropy, 40 / phonology.Entropy));
        }

        public string GenerateLetter(Random random)
        {
            return Orthography!.Transcribe(
                Phonology!.GenerateSyllable(random, false, out bool _), random).First();
        }

        public string GenerateWord(Random random)
        {
            var phonemes = new List<Phoneme>();
            bool requireOnset = false;
            for (int i=0; i<Math.Max(1, Math.Round(WordLengthSampler!.Generate(random))); ++i)
            {
                phonemes.AddRange(Phonology!.GenerateSyllable(random, requireOnset, out bool _));
            }
            return string.Join("", Orthography!.Transcribe(phonemes, random));
        }

        public override string ToString()
        {
            return string.Format("[Language]\n{0}\n{1}\n{2}", Phonetics, Orthography, Phonology);
        }
    }
}