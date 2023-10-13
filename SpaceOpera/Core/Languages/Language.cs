using Cardamom.Utils.Generators.Samplers;
using MathNet.Numerics.Distributions;

namespace SpaceOpera.Core.Languages
{
    public class Language
    {
        private static readonly float s_StdDevFactor = 0.2f;

        public Phonetics Phonetics { get; }
        public Orthography Orthography { get; }
        public Phonology Phonology { get; }

        public Language(Phonetics phonetics, Orthography orthography, Phonology phonology)
        {
            Phonetics = phonetics;
            Orthography = orthography;
            Phonology = phonology;
        }

        public string GenerateLetter(Random random)
        {
            return Orthography!.Transcribe(
                Phonology!.GenerateSyllable(random, false, out bool _), random).First();
        }

        public string GenerateWord(Random random, float bits)
        {
            var phonemes = new List<Phoneme>();
            bool requireOnset = false;
            var length = bits * Phonology.InvEntropy;
            for (int i=0; i<Math.Max(1, Math.Round(Normal.Sample(length, s_StdDevFactor * length))); ++i)
            {
                phonemes.AddRange(Phonology!.GenerateSyllable(random, requireOnset, out bool voidOffset));
                requireOnset = voidOffset;
            }
            return string.Join("", Orthography!.Transcribe(phonemes, random));
        }

        public override string ToString()
        {
            return string.Format("[Language]\n{0}\n{1}\n{2}", Phonetics, Orthography, Phonology);
        }
    }
}