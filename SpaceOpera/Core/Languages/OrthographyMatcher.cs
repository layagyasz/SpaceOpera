using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages
{
    class OrthographyMatcher
    {
        public Phoneme Pattern { get; }
        public string Symbol { get; }

        public OrthographyMatcher(Phoneme Pattern, string Symbol)
        {
            this.Pattern = Pattern;
            this.Symbol = Symbol;
        }

        public bool Matches(Phoneme Phoneme)
        {
            return Pattern == Phoneme;
        }
    }
}