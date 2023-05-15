using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.ImageProcessing;
using Cardamom.ImageProcessing.Filters;
using Cardamom.ImageProcessing.Pipelines;
using Cardamom.ImageProcessing.Pipelines.Nodes;
using Cardamom.Json;
using Cardamom.Utils.Generators.Generic;
using Cardamom.Utils.Suppliers;
using OpenTK.Mathematics;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Universe.Generator
{
    [JsonConverter(typeof(BuilderJsonConverter))]
    [BuilderClass(typeof(Builder))]
    public class StellarBodySurfaceGenerator
    {
        private static readonly float s_RoughnessDivisor = 4096f;

        private readonly Dictionary<string, IGenerator> _generators;
        private readonly Library<Cardamom.Utils.Suppliers.Generic.IConstantSupplier> _parameters;
        private readonly List<BiomeOption> _biomes;
        private readonly Pipeline _biomeIdPipeline;
        private readonly Pipeline _surfacePipeline;
        private readonly IConstantSupplier<Vector2> _scaleParameter;
        private readonly IConstantSupplier<float> _roughnessParameter;
        private readonly IConstantSupplier<IEnumerable<Classify.Classification>> _biomeParameter;
        private readonly IConstantSupplier<IEnumerable<Classify.Classification>> _surfaceDiffuseParameter;
        private readonly IConstantSupplier<IEnumerable<Classify.Classification>> _surfaceLightingParameter;

        private StellarBodySurfaceGenerator(
            Dictionary<string, IGenerator> generators,
            Library<Cardamom.Utils.Suppliers.Generic.IConstantSupplier> parameters,
            IEnumerable<BiomeOption> biomes,
            Pipeline biomeIdPipeline,
            Pipeline surfacePipeline,
            IConstantSupplier<Vector2> scaleParameter,
            IConstantSupplier<float> roughnessParameter,
            IConstantSupplier<IEnumerable<Classify.Classification>> biomeParameter,
            IConstantSupplier<IEnumerable<Classify.Classification>> surfaceDiffuseParameter,
            IConstantSupplier<IEnumerable<Classify.Classification>> surfaceLightingParameter)
        {
            _generators = generators;
            _parameters = parameters;
            _biomes = biomes.ToList();
            _biomeIdPipeline = biomeIdPipeline;
            _surfacePipeline = surfacePipeline;
            _scaleParameter = scaleParameter;
            _roughnessParameter = roughnessParameter;
            _biomeParameter = biomeParameter;
            _surfaceDiffuseParameter = surfaceDiffuseParameter;
            _surfaceLightingParameter = surfaceLightingParameter;
        }

        public Library<object> Generate(Random random)
        {
            return _generators.ToLibrary(x => x.Key, x => x.Value.Generate<object>(random));
        }

        public Biome[] Get(
            float temperature, 
            Library<object> parameterValues,
            Vector3[] positions,
            StellarBodySurfaceGeneratorResources resources)
        {
            var data = new Color4[resources.Resolution, resources.Resolution];
            for (int i=0; i<positions.Length; ++i)
            {
                var p = positions[i];
                data[i % resources.Resolution, i / resources.Resolution] = new(p.X, p.Y, p.Z, 1);
            }
            var canvases = resources.GetCanvasProvider();
            var input = canvases.Get();
            var tex = input.GetTexture();
            tex.Update(new(0, 0), data);
            foreach (var parameter in parameterValues)
            {
                _parameters[parameter.Key].Set(parameter.Value);
            }
            var biome = new List<Classify.Classification>();
            for (int i = 0; i < _biomes.Count; ++i)
            {
                var option = _biomes[i];
                if (_biomes[i].ThermalRange.Contains(temperature))
                {
                    biome.Add(CreateClassification(option, new(i, i, i, 1f)));
                }
            }
            _biomeParameter.Set(biome);
            var output = _biomeIdPipeline.Run(canvases, input);
            data = output[0].GetTexture().GetData();

            var result = new Biome[positions.Length];
            for (int i=0;i<positions.Length; ++i)
            {
                result[i] = _biomes[(int)data[i % resources.Resolution, i / resources.Resolution].R].Biome!;
            }
            canvases.Return(input);
            canvases.Return(output[0]);
            return result;
        }

        public Material GenerateSurface(
            float temperature,
            Dictionary<string, object> parameterValues, 
            Func<Biome, Color4> diffuseFn, 
            Func<Biome, Color4> lightingFn,
            StellarBodySurfaceGeneratorResources resources)
        {
            foreach (var parameter in parameterValues)
            {
                _parameters[parameter.Key].Set(parameter.Value);
            }

            var diffuse = new List<Classify.Classification>();
            var lighting = new List<Classify.Classification>();
            for (int i = 0; i < _biomes.Count; ++i)
            {
                var option = _biomes[i];
                if (option.ThermalRange.Contains(temperature))
                {
                    diffuse.Add(CreateClassification(option, diffuseFn(option.Biome!)));
                    lighting.Add(CreateClassification(option, lightingFn(option.Biome!)));
                }
            }
            _surfaceDiffuseParameter.Set(diffuse);
            _surfaceLightingParameter.Set(lighting);
            _scaleParameter.Set(new Vector2(1f / resources.Resolution, 1f / resources.Resolution));
            _roughnessParameter.Set(1f *  resources.Resolution / s_RoughnessDivisor);

            var surface = _surfacePipeline.Run(resources.GetCanvasProvider());
            return new(surface[0].GetTexture(), surface[2].GetTexture(), surface[1].GetTexture());
        }

        public bool IsHomogenous(float temperature)
        {
            return _biomes
                .Where(x => x.ThermalRange.Contains(temperature))
                .Aggregate(0, (x, y) => x | (y.Biome!.IsTraversable ? 2 : 1)) != 3;
        }

        private static Classify.Classification CreateClassification(BiomeOption option, Color4 color)
        {
            return new Classify.Classification()
            {
                Color = color,
                Center = option.Center,
                AxisWeight = option.AxisWeight,
                Weight = option.Weight,
                BlendRange = option.BlendRange,
            };
        }

        public class Builder
        {
            public Dictionary<string, IGenerator> Generators { get; set; } = new();
            public Library<Cardamom.Utils.Suppliers.Generic.IConstantSupplier> Parameters { get; set; } 
                = new();
            public List<BiomeOption> BiomeOptions { get; set; } = new();
            public Pipeline.Builder? Pipeline { get; set; }
            public string? HeightOutput { get; set; }
            public Channel HeightChannel { get; set; }

            public StellarBodySurfaceGenerator Build()
            {
                var biomePipeline = Pipeline!.Clone();
                biomePipeline.AddNode(new InputNode.Builder().SetKey("position").SetIndex(0));
                var biomeParameter = new ConstantSupplier<IEnumerable<Classify.Classification>>();
                biomePipeline.AddNode(
                    new ClassifyNode.Builder()
                        .SetKey("biome")
                        .SetChannel(Channel.Color)
                        .SetInput("input", "output")
                        .SetParameters(
                            new ClassifyNode.Parameters()
                            {
                                Classifications = biomeParameter
                            }));
                biomePipeline.AddOutput("biome");

                var scaleParameter = new ConstantSupplier<Vector2>();
                var roughnessParameter = new ConstantSupplier<float>();
                var diffuseParameter = new ConstantSupplier<IEnumerable<Classify.Classification>>();
                var lightingParameter = new ConstantSupplier<IEnumerable<Classify.Classification>>();
                var surfacePipeline = Pipeline!.Clone();
                surfacePipeline
                    .AddNode(new GeneratorNode.Builder().SetKey("new"))
                    .AddNode(
                        new GradientNode.Builder()
                            .SetKey("gradient")
                            .SetChannel(Channel.Red | Channel.Green)
                            .SetInput("input", "new")
                            .SetParameters(
                                new GradientNode.Parameters()
                                {
                                    Scale = scaleParameter,
                                    Gradient = ConstantSupplier<Matrix4x2>.Create(
                                        new Matrix4x2(new(1, 0), new(0, 1), new(), new()))
                                }))
                    .AddNode(
                        new SpherizeNode.Builder()
                            .SetKey("position")
                            .SetChannel(Channel.All)
                            .SetInput("input", "gradient"))
                    .AddNode(
                        new ClassifyNode.Builder()
                            .SetKey("diffuse")
                            .SetChannel(Channel.All)
                            .SetInput("input", "output")
                            .SetParameters(
                                new ClassifyNode.Parameters()
                                {
                                    Blend = new ConstantSupplier<bool>(true),
                                    Classifications = diffuseParameter
                                }))
                    .AddNode(
                        new ClassifyNode.Builder()
                            .SetKey("lighting")
                            .SetChannel(Channel.All)
                            .SetInput("input", "output")
                            .SetParameters(
                                new ClassifyNode.Parameters()
                                { 
                                    Blend = new ConstantSupplier<bool>(true),
                                    Classifications = lightingParameter
                                }))
                    .AddOutput("diffuse")
                    .AddOutput("lighting");
                if (HeightOutput != null && HeightChannel != Channel.None)
                {
                    surfacePipeline
                        .AddNode(
                            new SobelNode.Builder()
                                .SetKey("normal")
                                .SetChannel(HeightChannel)
                                .SetInput("input", HeightOutput)
                                .SetParameters(
                                    new SobelNode.Parameters()
                                    {
                                        Roughness = roughnessParameter
                                    }));
                }
                else
                {
                    surfacePipeline.AddNode(new GeneratorNode.Builder().SetKey("normal"));
                }
                surfacePipeline.AddOutput("normal");

                return new StellarBodySurfaceGenerator(
                    Generators,
                    Parameters,
                    BiomeOptions,
                    biomePipeline.Build(),
                    surfacePipeline.Build(),
                    scaleParameter,
                    roughnessParameter,
                    biomeParameter,
                    diffuseParameter,
                    lightingParameter);
            }
        }
    }
}
