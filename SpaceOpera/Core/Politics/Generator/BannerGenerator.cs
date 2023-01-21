namespace SpaceOpera.Core.Politics.Generator
{
    public class BannerGenerator
    {
        public int Symbols { get; set; }
        public int Backgrounds { get; set; }
        public int Colors { get; set; }

        public Banner Generate(Random random)
        {
            return GenerateWithSymbol(random.Next(Symbols), random);
        }

        public IEnumerable<Banner> GenerateUnique(int count,  Random random)
        {
            int[] symbols = GenerateUnique(Symbols, count, random);
            foreach (int symbol in symbols)
            {
                yield return GenerateWithSymbol(symbol, random);
            }
        }

        private Banner GenerateWithSymbol(int symbol, Random random)
        {
            int[] colors = GenerateUnique(3, Colors, random);
            return new(symbol, random.Next(Backgrounds), colors[0], colors[1], colors[2]);
        }

        private static int[] GenerateUnique(int count, int max, Random random)
        {
            int[] result = new int[count];
            for (int i=0; i<count; ++i)
            {
                int value;
                do
                {
                    value = random.Next(max);
                }
                while (Array.IndexOf(result, value) < i);
                result[i] = value;
            }
            return result;
        }
    }
}
