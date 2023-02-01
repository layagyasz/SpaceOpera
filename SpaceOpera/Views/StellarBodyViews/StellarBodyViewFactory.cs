using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Mathematics.Coordinates.Projections;
using Cardamom.Mathematics.Coordinates;
using Cardamom.Mathematics.Geometry;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Generator;
using OpenTK.Graphics.OpenGL4;
using SpaceOpera.Core.Universe.Spectra;
using Cardamom.Mathematics.Color;

namespace SpaceOpera.Views.StellarBodyViews
{
    public class StellarBodyViewFactory : GraphicsResource
    {
        private readonly static int s_SphereHighResSubdivisions = 64;

        private VertexBuffer<VertexLit3>? _sphereHighRes;

        public Dictionary<Biome, BiomeRenderDetails> BiomeRenderDetails { get; }
        public Library<StellarBodyGenerator> StellarBodyGenerators { get; }
        public RenderShader SurfaceShader { get; }
        public SpectrumSensitivity HumanEyeSensitivity { get; }

        public StellarBodyViewFactory(
            Dictionary<Biome, BiomeRenderDetails> biomeRenderDetails,
            Library<StellarBodyGenerator> stellarBodyGenerators,
            RenderShader surfaceShader, 
            SpectrumSensitivity humanEyeSensitivity)
        {
            BiomeRenderDetails = biomeRenderDetails;
            StellarBodyGenerators = stellarBodyGenerators;
            SurfaceShader = surfaceShader;
            HumanEyeSensitivity = humanEyeSensitivity;
        }

        public StellarBodyModel GenerateModel(StellarBody stellarBody)
        {
            var spectrum = new BlackbodySpectrum(stellarBody.Orbit.Focus.Temperature);
            var peakWavelength =
                Math.Min(
                    HumanEyeSensitivity.Range.Maximum - 1,
                    Math.Max(
                        spectrum.GetPeak(),
                        HumanEyeSensitivity.Range.Minimum + 1));
            var peakColor = ToColor(HumanEyeSensitivity.GetColor(peakWavelength));

            var material = StellarBodyGenerators[stellarBody.Type]
                .SurfaceGenerator!
                    .GenerateSurface(
                        stellarBody.Parameters,
                        x => BiomeRenderDetails[x].GetColor(peakColor),
                        x => BiomeRenderDetails[x].GetLighting());
            var buffer = GenerateSphere(s_SphereHighResSubdivisions);
            return new StellarBodyModel(new(buffer, SurfaceShader, material));
        }

        protected override void DisposeImpl()
        {
            _sphereHighRes?.Dispose();
            _sphereHighRes = null;
        }

        private static Color4 ToColor(ColorCie color)
        {
            return ColorSystem.Ntsc.Transform(color);
        }

        private static VertexBuffer<VertexLit3> GenerateSphere(int subdivisions)
        {
            var uvSphereSolid = Solid<Spherical3>.GenerateSphericalUvSphere(1, subdivisions);
            VertexLit3[] vertices = new VertexLit3[6 * uvSphereSolid.Faces.Length];
            var projection = new CylindricalProjection.Spherical();
            for (int i = 0; i < uvSphereSolid.Faces.Length; ++i)
            {
                for (int j = 0; j < uvSphereSolid.Faces[i].Vertices.Length; ++j)
                {
                    var vert = uvSphereSolid.Faces[i].Vertices[j];
                    var texCoords = new Vector2(0.5f, 1) * projection.Project(vert);
                    var c = vert.AsCartesian();
                    vertices[6 * i + j] = new(c, Color4.White, texCoords, c.Normalized(), texCoords, texCoords);
                }
            }
            return new(vertices, PrimitiveType.Triangles);
        }
    }
}
