using OpenTK.Mathematics;

namespace SpaceOpera.Core.Politics.Generator
{
    public class BannerGenerator
    {
        public int Symbols { get; set; }
        public int Patterns { get; set; }
        public Color4[] Colors { get; set; } = Array.Empty<Color4>();

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
            int[] colors = GenerateUnique(3, Colors.Length, random);
            return new(symbol, random.Next(Patterns), Colors[colors[0]], Colors[colors[1]], Colors[colors[2]]);
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
