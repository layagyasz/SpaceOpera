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

        private readonly Library<IGenerator> _generators;
        private readonly Library<Cardamom.Utils.Suppliers.Generic.IConstantSupplier> _parameters;
        private readonly List<Biome> _biomes;
        private readonly Pipeline _biomeIdPipeline;

        public StellarBodySurfaceGenerator(
            Library<IGenerator> generators,
            Library<Cardamom.Utils.Suppliers.Generic.IConstantSupplier> parameters,
            IEnumerable<Biome> biomes, 
            Pipeline biomeIdPipeline)
        {
            _generators = generators;
            _parameters = parameters;
            _biomes = biomes.ToList();
            _biomeIdPipeline = biomeIdPipeline;
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
                result[i] = _biomes[(int)data[i % s_TexSize, i / s_TexSize].R];
            }
            s_BiomeCanvasProvider.Return(output[0]);
            return result;
        }

        public class Builder
        {
            public Library<IGenerator> Generators { get; set; } = new();
            public Library<Cardamom.Utils.Suppliers.Generic.IConstantSupplier> Parameters { get; set; } = new();
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
                                Minimum = condition.Minimum, 
                                Maximum = condition.Maximum
                            });
                    }
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

                return new StellarBodySurfaceGenerator(
                    Generators, Parameters, BiomeOptions.Select(x => x.Biome!), biomePipeline.Build());
            }
        }
    }
}
