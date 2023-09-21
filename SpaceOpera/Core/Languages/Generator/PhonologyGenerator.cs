using Cardamom.Trackers;

namespace SpaceOpera.Core.Languages.Generator
{
    public class PhonologyGenerator
    {
        public List<Frequent<List<PhonologyExclusion>>> Exclusions { get; set; } = new();

        public Phonology Generate(Phonetics phonetics, GeneratorContext context)
        {
            var builder = new Phonology.Builder(phonetics.Phonemes);
            foreach (var exclusionSet in Exclusions.Where(x => context.Random.NextSingle() < x.Frequency))
            {
                foreach (var exclusion in exclusionSet.Value!)
                {
                    switch (exclusion.Segment)
                    {
                        case PhonologyExclusion.PhonologySegmentType.Onset:
                            builder.AddOnsetExclusion(exclusion.Left, exclusion.Right);
                            break;
                        case PhonologyExclusion.PhonologySegmentType.Nucleus:
                            builder.AddNucleusExclusion(exclusion.Left, exclusion.Right);
                            break;
                        case PhonologyExclusion.PhonologySegmentType.Offset:
                            builder.AddOffsetExclusion(exclusion.Left, exclusion.Right);
                            break;
                    }
                }
            }
            return builder.Build();
        }
    }
}