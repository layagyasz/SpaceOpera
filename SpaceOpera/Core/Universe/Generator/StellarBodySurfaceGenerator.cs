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
        private static readonly int s_TexSize = 64;
        private static Texture? s_PositionTex;
        private static readonly ICanvasProvider s_BiomeCanvasProvider = 
            new CachingCanvasProvider(new(s_TexSize, s_TexSize), Color4.Black);

        private static readonly int s_SurfaceSize = 1024;
        private static ICanvasProvider? s_SurfaceCanvaseProvider;
            

        private readonly Dictionary<string, IGenerator> _generators;
        private readonly Library<Cardamom.Utils.Suppliers.Generic.IConstantSupplier> _parameters;
        private readonly List<BiomeOption> _biomes;
        private readonly Pipeline _biomeIdPipeline;
        private readonly Pipeline _surfacePipeline;
        private readonly IConstantSupplier<IEnumerable<Classify.Classification>> _surfaceClassificationParameter;

        private StellarBodySurfaceGenerator(
            Dictionary<string, IGenerator> generators,
            Library<Cardamom.Utils.Suppliers.Generic.IConstantSupplier> parameters,
            IEnumerable<BiomeOption> biomes, 
            Pipeline biomeIdPipeline,
            Pipeline surfacePipeline,
            IConstantSupplier<IEnumerable<Classify.Classification>> surfaceClassificationParameter)
        {
            _generators = generators;
            _parameters = parameters;
            _biomes = biomes.ToList();
            _biomeIdPipeline = biomeIdPipeline;
            _surfacePipeline = surfacePipeline;
            _surfaceClassificationParameter = surfaceClassificationParameter;
        }

        public Library<object> Generate(Random random)
        {
            return _generators.ToLibrary(x => x.Key, x => x.Value.Generate<object>(random));
        }

        public Biome[] Get(Library<object> parameterValues, Vector3[] positions)
        {
            var data = new Color4[s_TexSize, s_TexSize];
            for (int i=0; i<positions.Length; ++i)
            {
                var p = positions[i];
                data[i % s_TexSize, i / s_TexSize] = new(p.X, p.Y, p.Z, 1);
            }
            s_PositionTex ??= Texture.Create(new(s_TexSize, s_TexSize));
            s_PositionTex.Update(new(0, 0), data);
            foreach (var parameter in parameterValues)
            {
                _parameters[parameter.Key].Set(parameter.Value);
            }
            var output = _biomeIdPipeline.Run(s_BiomeCanvasProvider, new Canvas(s_PositionTex));
            data = output[0].GetTexture().GetData();

            var result = new Biome[positions.Length];
            for (int i=0;i<positions.Length; ++i)
            {
                result[i] = _biomes[(int)data[i % s_TexSize, i / s_TexSize].R].Biome!;
            }
            s_BiomeCanvasProvider.Return(output[0]);
            return result;
        }

        public Canvas GenerateSurface(Dictionary<string, object> parameterValues, Func<Biome, Color4> biomeColorFn)
        {
            foreach (var parameter in parameterValues)
            {
                _parameters[parameter.Key].Set(parameter.Value);
            }

            var classifications = new List<Classify.Classification>();
            for (int i = 0; i < _biomes.Count; ++i)
            {
                var option = _biomes[i];
                var classification = new Classify.Classification()
                {
                    Color = biomeColorFn(option.Biome!)
                };
                foreach (var condition in option.Conditions)
                {
                    classification.Conditions.Add(
                        new Classify.Condition()
                        {
                            Channel = condition.Channel,
                            Range = condition.Range
                        });
                }
                classifications.Add(classification);
            }
            _surfaceClassificationParameter.Set(classifications);

            s_SurfaceCanvaseProvider ??= new CachingCanvasProvider(new(s_SurfaceSize, s_SurfaceSize), Color4.Black);
            return _surfacePipeline.Run(s_SurfaceCanvaseProvider)[0];
        }

        public class Builder
        {
            public Dictionary<string, IGenerator> Generators { get; set; } = new();
            public Library<Cardamom.Utils.Suppliers.Generic.IConstantSupplier> Parameters { get; set; } 
                = new();
            public List<BiomeOption> BiomeOptions { get; set; } = new();
            public Pipeline.Builder? Pipeline { get; set; }

            public StellarBodySurfaceGenerator Build()
            {
                var biomePipeline = Pipeline!.Clone();
                biomePipeline.AddNode(new InputNode.Builder().SetKey("position").SetIndex(0));
                var classifications = new List<Classify.Classification>();
                for (int i=0; i<BiomeOptions!.Count; ++i)
                {
                    var option = BiomeOptions[i];
                    var classification = new Classify.Classification()
                    {
                        Color = new(i, i, i, 1)
                    };
                    foreach (var condition in option.Conditions)
                    {
                        classification.Conditions.Add(
                            new Classify.Condition() 
                            { 
                                Channel = condition.Channel, 
                                Range = condition.Range
                            });
                    }
                    classifications.Add(classification);
                }
                biomePipeline.AddNode(
                    new ClassifyNode.Builder()
                        .SetKey("biome")
                        .SetChannel(Channel.Color)
                        .SetInput("input", "output")
                        .SetParameters(
                            new ClassifyNode.Parameters()
                            {
                                Classifications = 
                                    ConstantSupplier<IEnumerable<Classify.Classification>>
                                        .Create<IEnumerable<Classify.Classification>>(classifications)
                            }));
                biomePipeline.AddOutput("biome");

                var classificationParameter = new ConstantSupplier<IEnumerable<Classify.Classification>>();
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
                                    Scale = ConstantSupplier<Vector2>.Create(
                                        new Vector2(1f / s_SurfaceSize, 1f / s_SurfaceSize)),
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
                                    Classifications = classificationParameter
                                }))
                    .AddOutput("diffuse");

                return new StellarBodySurfaceGenerator(
                    Generators,
                    Parameters,
                    BiomeOptions,
                    biomePipeline.Build(), 
                    surfacePipeline.Build(), 
                    classificationParameter);
            }
        }
    }
}
