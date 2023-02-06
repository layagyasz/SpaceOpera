﻿using Cardamom.Collections;
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
using SharpFont;

namespace SpaceOpera.Views.StellarBodyViews
{
    public class StellarBodyViewFactory : GraphicsResource
    {
        private readonly static float s_AtmosphericScattering = 4f;
        private readonly static int s_SphereHighResSubdivisions = 64;

        private VertexBuffer<VertexLit3>? _sphereHighResSurf;
        private VertexBuffer<VertexLit3>? _sphereHighResAtmo;

        public Dictionary<Biome, BiomeRenderDetails> BiomeRenderDetails { get; }
        public Library<StellarBodyGenerator> StellarBodyGenerators { get; }
        public RenderShader SurfaceShader { get; }
        public RenderShader AtmosphereShader { get; }
        public SpectrumSensitivity HumanEyeSensitivity { get; }

        public StellarBodyViewFactory(
            Dictionary<Biome, BiomeRenderDetails> biomeRenderDetails,
            Library<StellarBodyGenerator> stellarBodyGenerators,
            RenderShader surfaceShader,
            RenderShader atmosphereShader,
            SpectrumSensitivity humanEyeSensitivity)
        {
            BiomeRenderDetails = biomeRenderDetails;
            StellarBodyGenerators = stellarBodyGenerators;
            SurfaceShader = surfaceShader;
            AtmosphereShader = atmosphereShader;
            HumanEyeSensitivity = humanEyeSensitivity;
        }

        public StellarBodyModel CreateModel(StellarBody stellarBody)
        {
            var spectrum = new BlackbodySpectrum(stellarBody.Orbit.Focus.Temperature);
            var peakWavelength =
                Math.Min(
                    HumanEyeSensitivity.Range.Maximum - 1,
                    Math.Max(
                        spectrum.GetPeak(),
                        HumanEyeSensitivity.Range.Minimum + 1));
            var peakColor = ToColor(HumanEyeSensitivity.GetColor(peakWavelength));
            var scatteredColor = ToColor(HumanEyeSensitivity.GetColor(new RayleighScatteredSpectrum(spectrum)));

            var material = StellarBodyGenerators[stellarBody.Type]
                .SurfaceGenerator!
                    .GenerateSurface(
                        stellarBody.Parameters,
                        x => BiomeRenderDetails[x].GetColor(peakColor, scatteredColor),
                        x => BiomeRenderDetails[x].GetLighting());
            _sphereHighResSurf ??= CreateSphere(1f, s_SphereHighResSubdivisions, Color4.White);
            _sphereHighResAtmo ??= 
                CreateSphere(
                    1.2f, 
                    s_SphereHighResSubdivisions,
                    new Color4(
                        s_AtmosphericScattering * 0.1066f,
                        s_AtmosphericScattering * 0.3244f, 
                        s_AtmosphericScattering * 0.6830f,
                        1f));
            return new StellarBodyModel(
                new(_sphereHighResSurf, SurfaceShader, material), _sphereHighResAtmo, AtmosphereShader);
        }

        protected override void DisposeImpl()
        {
            _sphereHighResSurf?.Dispose();
            _sphereHighResSurf = null;

            _sphereHighResAtmo?.Dispose();
            _sphereHighResAtmo = null;
        }

        private static Color4 ToColor(ColorCie color)
        {
            return ColorSystem.Ntsc.Transform(color);
        }

        private static VertexBuffer<VertexLit3> CreateSphere(float scale, int subdivisions, Color4 color)
        {
            var uvSphereSolid = Solid<Spherical3>.GenerateSphericalUvSphere(scale, subdivisions);
            VertexLit3[] vertices = new VertexLit3[6 * uvSphereSolid.Faces.Length];
            var projection = new CylindricalProjection.Spherical();
            for (int i = 0; i < uvSphereSolid.Faces.Length; ++i)
            {
                for (int j = 0; j < uvSphereSolid.Faces[i].Vertices.Length; ++j)
                {
                    var vert = uvSphereSolid.Faces[i].Vertices[j];
                    var texCoords = new Vector2(0.5f, 1) * projection.Project(vert);
                    var c = vert.AsCartesian();
                    vertices[6 * i + j] = new(c, color, texCoords, c.Normalized(), texCoords, texCoords);
                }
            }
            return new(vertices, PrimitiveType.Triangles);
        }
    }
}
