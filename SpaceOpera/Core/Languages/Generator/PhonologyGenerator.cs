using Cardamom.Trackers;

namespace SpaceOpera.Core.Languages.Generator
{
    public class PhonologyGenerator
    {
        public List<Frequent<PhonologyExclusion>> Exclusions { get; set; } = new();

        public Phonology Generate(Phonetics phonetics, GeneratorContext context)
        {
            var builder = new Phonology.Builder(phonetics.Phonemes);
            foreach (var exclusion in Exclusions.Where(x => context.Random.NextSingle() < x.Frequency))
            {
                switch (exclusion.Value!.Segment)
                {
                    case PhonologyExclusion.PhonologySegmentType.Onset:
                        builder.AddOnsetExclusion(exclusion.Value.Left, exclusion.Value.Right);
                        break;
                    case PhonologyExclusion.PhonologySegmentType.Nucleus:
                        builder.AddNucleusExclusion(exclusion.Value.Left, exclusion.Value.Right);
                        break;
                    case PhonologyExclusion.PhonologySegmentType.Offset:
                        builder.AddOffsetExclusion(exclusion.Value.Left, exclusion.Value.Right);
                        break;
                }
            }
            return builder.Build();
        }
    }
}