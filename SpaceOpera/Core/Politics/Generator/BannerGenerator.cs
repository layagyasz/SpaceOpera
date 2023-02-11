namespace SpaceOpera.Core.Politics.Generator
{
    public class BannerGenerator
    {
        public int Symbols { get; set; }
        public int Backgrounds { get; set; }
        public int Colors { get; set; }

        public Banner Generate(GeneratorContext context)
        {
            return GenerateWithSymbol(context.Random.Next(Symbols), context.Random);
        }

        public IEnumerable<Banner> GenerateUnique(int count,  GeneratorContext context)
        {
            int[] symbols = GenerateUnique(count, Symbols, context.Random);
            foreach (int symbol in symbols)
            {
                yield return GenerateWithSymbol(symbol, context.Random);
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
            int j;
            for (int i=0; i<count; ++i)
            {
                int value;
                do
                {
                    value = random.Next(max);
                    j = Array.IndexOf(result, value);
                }
                while (j < i && j > -1);
                result[i] = value;
            }
            return result;
        }
    }
}
