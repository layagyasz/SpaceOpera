using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages.Generator
{
    class PhonologyGenerator
    {
        public List<Frequent<PhonologyExclusion>> Exclusions { get; set; }

        public Phonology Generate(Phonetics Phonetics, Random Random)
        {
            var builder = new Phonology.Builder(Phonetics.Phonemes);
            foreach (var exclusion in Exclusions.Where(x => Random.NextDouble() < x.Frequency))
            {
                switch (exclusion.Value.Segment)
                {
                    case PhonologyExclusion.PhonologySegmentType.ONSET:
                        builder.AddOnsetExclusion(exclusion.Value.Left, exclusion.Value.Right);
                        break;
                    case PhonologyExclusion.PhonologySegmentType.NUCLEUS:
                        builder.AddNucleusExclusion(exclusion.Value.Left, exclusion.Value.Right);
                        break;
                    case PhonologyExclusion.PhonologySegmentType.OFFSET:
                        builder.AddOffsetExclusion(exclusion.Value.Left, exclusion.Value.Right);
                        break;
                }
            }
            return builder.Build();
        }
    }
}