using Cardamom;
using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.BannerViews;

namespace SpaceOpera.View.Icons
{
    public class IconFactory : IIconDisposer
    {
        private static EnumMap<IconResolution, Vector2i> s_Resolution =
            new()
            {
                { IconResolution.Low, new(64, 64) },
                { IconResolution.High, new(512, 512) }
            };

        private class IconCamera : ICamera
        {
            private readonly Projection _projection = new(-1, Matrix4.CreateOrthographicOffCenter(0, 1, 0, 1, -1, 1));

            public EventHandler<EventArgs>? Changed { get; set; }

            public Vector3 Position => new();
            public float AspectRatio => 1f;

            public void SetAspectRatio(float aspectRatio)
            {
                throw new NotSupportedException();
            }

            public Matrix4 GetViewMatrix()
            {
                return Matrix4.Identity;
            }

            public Projection GetProjection()
            {
                return _projection;
            }
        }

        private class IconConfig : IRenderable
        {
            private readonly List<IconLayer> _layers;
            private readonly UiElementFactory _uiElementFactory;
            private readonly RenderShader _shader;

            public IconConfig(List<IconLayer> layers, UiElementFactory uiElementFactory, RenderShader shader)
            {
                _layers = layers;
                _uiElementFactory = uiElementFactory;
                _shader = shader;
            }

            public void Draw(IRenderTarget target, IUiContext context)
            {
                foreach (var layer in _layers)
                {
                    var tex = _uiElementFactory.GetTexture(layer.Texture);
                    var vertices = new Vertex3[layer.Vertices.Length];
                    for (int i = 0; i < vertices.Length; ++i)
                    {
                        vertices[i] =
                            new(
                                new(layer.Vertices[i]),
                                layer.Color,
                                tex.TextureView.Min + Utils.UnitTriangles[i] * tex.TextureView.Size);
                    }
                    target.Draw(
                        vertices, 
                        PrimitiveType.Triangles,
                        0, 
                        vertices.Length,
                        new(BlendMode.Alpha, _shader, tex.Texture!));
                }
            }

            public void Initialize() { }

            public void ResizeContext(Vector3 bounds) { }

            public void Update(long delta) { }
        }

        private readonly BannerViewFactory _bannerViewFactory;
        private readonly StellarBodyIconFactory _stellarBodyIconFactory;
        private readonly Library<IconAtom> _atoms;
        private readonly EnumMap<ComponentType, DesignedComponentIconConfig> _configs;
        private readonly Dictionary<Type, Func<object, IEnumerable<IconLayer>>> _definitionMap;
        private readonly UiElementFactory _uiElementFactory;
        private readonly RenderShader _shader;

        private readonly IconCache _cache = new();
        private readonly EnumMap<IconResolution, RenderTexture> _rasterTextures = 
            s_Resolution.ToEnumMap(x => x.Key, x => new RenderTexture(x.Value));

        public IconFactory(
            BannerViewFactory bannerViewFactory,
            StellarBodyIconFactory stellarBodyIconFactory,
            Library<IconAtom> atoms,
            EnumMap<ComponentType, DesignedComponentIconConfig> configs,
            UiElementFactory uiElementFactory)
        {
            _bannerViewFactory = bannerViewFactory;
            _stellarBodyIconFactory = stellarBodyIconFactory;
            _atoms = atoms;
            _configs = configs;
            _definitionMap = new()
            {
                { typeof(Banner), GetBannerDefinition },
                { typeof(BaseComponent),  GetAtomicDefinition },
                { typeof(BaseMaterial), GetAtomicDefinition },
                { typeof(BattalionTemplate), GetDesignedComponentDefinition },
                { typeof(Design), GetDesignDefinition },
                { typeof(DesignedComponent), GetDesignedComponentDefinition },
                { typeof(Division), GetDivisionDefinition },
                { typeof(DivisionTemplate), GetDesignedComponentDefinition },
                { typeof(Recipe), GetRecipeDefinition },
                { typeof(Structure), GetAtomicDefinition },
                { typeof(Unit), GetDesignedComponentDefinition },
            };
            _uiElementFactory = uiElementFactory;
            _shader = _uiElementFactory.GetShader("shader-default");
        }

        public Icon Create(
            Class @class,
            IElementController controller,
            object @object, 
            IconResolution resolution = IconResolution.Low)
        {
            var key = CompositeKey<object, IconResolution>.Create(GetKey(@object), resolution);
            if (_cache.TryGetTexture(key, out var texture))
            {
                return new(
                    key,
                    @class,
                    controller,
                    Color4.White,
                    texture!,
                    new(new(), s_Resolution[resolution]),
                    _shader,
                    this);
            }
            if (@object is StellarBody stellarBody)
            {
                var rasterTexture = _rasterTextures[resolution];
                rasterTexture.Clear();
                _stellarBodyIconFactory.Rasterize(stellarBody, rasterTexture);
                rasterTexture.Display();
                var tex = rasterTexture.CopyTexture();
                _cache.Put(key, tex);
                return new(
                    key, @class, controller, Color4.White, tex, new(new(), s_Resolution[resolution]), _shader, this);
            }
            var definition = GetDefinition(@object).ToList();
            if (definition.Count == 1)
            {
                var d = definition.First();
                var tex = _uiElementFactory.GetTexture(d.Texture);
                return new(key, @class, controller, d.Color, tex.Texture!, tex.TextureView, _shader, null);
            }
            else 
            {
                var tex = 
                    Rasterize(new IconConfig(definition, _uiElementFactory, _shader), new IconCamera(), resolution);
                _cache.Put(key, tex);
                return new(
                    key, @class, controller, Color4.White, tex, new(new(), s_Resolution[resolution]), _shader, this);
            }
        }

        public void Dispose(Icon icon)
        {
            _cache.Return(icon.Key);
        }

        public IEnumerable<IconLayer> GetDefinition(object @object)
        {
            return _definitionMap[@object.GetType()](@object);
        }

        private object GetKey(object @object)
        {
            if (@object is Army army)
            {
                return army.Faction.Banner;
            }
            if (@object is IFormationDriver driver)
            {
                return GetKey(driver.Formation);
            }
            if (@object is Faction faction)
            {
                return faction.Banner;
            }
            if (@object is Fleet fleet)
            {
                return fleet.Faction.Banner;
            }
            if (@object is Recipe recipe)
            {
                return recipe.Transformation.First(x => x.Value > 0).Key;
            }
            if (@object is UnitGrouping unitGrouping)
            {
                return unitGrouping.Unit;
            }
            return @object;
        }

        private IEnumerable<IconLayer> GetAtomicDefinition(object @object)
        {
            var key = @object as IKeyed;
            yield return _atoms[key!.Key].ToDefinition();
        }

        private IEnumerable<IconLayer> GetBannerDefinition(object @object)
        {
            return _bannerViewFactory.Create((Banner)@object);
        }

        private IEnumerable<IconLayer> GetDesignDefinition(object @object)
        {
            var design = (Design)@object;
            return GetDefinition(design.Components.First());
        }

        private IEnumerable<IconLayer> GetDesignedComponentDefinition(object @object)
        {
            return GetDesignedComponentDefinition(@object, new(Color4.White, Color4.Black, Color4.Red));
        }

        private IEnumerable<IconLayer> GetDesignedComponentDefinition(object @object, BannerColorSet colors)
        {
            var component = (DesignedComponent)@object;
            return _configs[component.Slot.Type].CreateDefinition(component, colors, this);
        }

        private IEnumerable<IconLayer> GetDivisionDefinition(object @object)
        {
            var division = (Division)@object;
            return GetDesignedComponentDefinition(division.Template, _bannerViewFactory.Get(division.Faction.Banner));
        }

        private IEnumerable<IconLayer> GetRecipeDefinition(object @object)
        {
            var recipe = (Recipe)@object;
            return GetDefinition(recipe.Transformation.First(x => x.Value > 0).Key);
        }

        private Texture Rasterize(IRenderable renderable, ICamera camera, IconResolution resolution)
        {
            var rasterTexture = _rasterTextures[resolution];
            rasterTexture.Clear();
            rasterTexture.PushModelMatrix(Matrix4.Identity);
            rasterTexture.PushViewMatrix(camera.GetViewMatrix());
            rasterTexture.PushProjection(camera.GetProjection());
            renderable.Draw(rasterTexture, new SimpleUiContext());
            rasterTexture.PopProjectionMatrix();
            rasterTexture.PopViewMatrix();
            rasterTexture.PopModelMatrix();
            rasterTexture.Display();
            return rasterTexture.CopyTexture();
        }
    }
}
