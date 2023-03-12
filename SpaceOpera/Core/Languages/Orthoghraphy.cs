namespace SpaceOpera.Core.Languages
{
    public class Orthography
    {
        public List<OrthographyMatcher> OrthographyMatchers { get; }

        public Orthography(IEnumerable<OrthographyMatcher> orthographyMatchers)
        {
            OrthographyMatchers = orthographyMatchers.ToList();
        }

        public IEnumerable<string> Transcribe(List<Phoneme> phonemes, Random random)
        {
            foreach (var phoneme in phonemes)
            {
                var options = OrthographyMatchers.Where(x => x.Matches(phoneme)).ToList();
                yield return options[random.Next(0, options.Count)].Symbol;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "[Orthography]\n{0}",
                string.Join(
                    "\n", OrthographyMatchers.Select(x => string.Format("{0} => {1}", x.Pattern, x))));
        }
    }
}