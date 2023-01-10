using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages
{
    class Phonetics
    {
        public List<Frequent<Phoneme>> Phonemes { get; }

        public Phonetics(IEnumerable<Frequent<Phoneme>> Phonemes)
        {
            this.Phonemes = Phonemes.ToList();
        }

        public override string ToString()
        {
            return string.Format(
                "[Phonetics]\n{0}",
                string.Join("\n", Phonemes.Select(x => string.Format("{0} {1}", x.Value, x.Frequency))));
        }
    }
}