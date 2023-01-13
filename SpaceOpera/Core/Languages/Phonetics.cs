using Cardamom.Trackers;

namespace SpaceOpera.Core.Languages
{
    public class Phonetics
    {
        public List<Frequent<Phoneme>> Phonemes { get; }

        public Phonetics(IEnumerable<Frequent<Phoneme>> phonemes)
        {
            Phonemes = phonemes.ToList();
        }

        public override string ToString()
        {
            return string.Format(
                "[Phonetics]\n{0}",
                string.Join("\n", Phonemes.Select(x => string.Format("{0} {1}", x.Value, x.Frequency))));
        }
    }
}