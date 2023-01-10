using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages
{
    class Orthography
    {
        public List<OrthographyMatcher> OrthographyMatchers { get; }

        public Orthography(IEnumerable<OrthographyMatcher> OrthographyMatchers)
        {
            this.OrthographyMatchers = OrthographyMatchers.ToList();
        }

        public IEnumerable<string> Transcribe(List<Phoneme> Phonemes, Random Random)
        {
            foreach (var phoneme in Phonemes)
            {
                var options = OrthographyMatchers.Where(x => x.Matches(phoneme)).ToList();
                yield return options[Random.Next(0, options.Count)].Symbol;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "[Orthography]\n{0}",
                string.Join(
                    "\n", OrthographyMatchers.Select(x => string.Format("{0} => {1}", x.Pattern.Symbol, x.Symbol))));
        }
    }
}