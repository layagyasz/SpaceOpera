using Cardamom.Collections;
using Cardamom.ImageProcessing;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Generator;

namespace SpaceOpera.Views.StellarBodyViews
{
    public class StellarBodyViewFactory
    {
        public Dictionary<Biome, BiomeRenderDetails> BiomeRenderDetails { get; set; }
        public Library<StellarBodyGenerator> StellarBodyGenerators { get; set; }

        public StellarBodyViewFactory(
            Dictionary<Biome, BiomeRenderDetails> biomeRenderDetails,
            Library<StellarBodyGenerator> stellarBodyGenerators)
        {
            BiomeRenderDetails = biomeRenderDetails;
            StellarBodyGenerators = stellarBodyGenerators;
        }

        public Canvas GenerateSurfaceFor(StellarBody StellarBody)
        {
            return StellarBodyGenerators[StellarBody.Type]
                .SurfaceGenerator!.GenerateSurface(StellarBody.Parameters, x => BiomeRenderDetails[x].Color);
        }
    }
}
