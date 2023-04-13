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
using SpaceOpera.View.FactionViews;

namespace SpaceOpera.View.Icons
{
    public class IconFactory
    {
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

            public IconConfig(IEnumerable<IconLayer> layers, UiElementFactory uiElementFactory, RenderShader shader)
            {
                _layers = layers.ToList();
                _uiElementFactory = uiElementFactory;
                _shader = shader;
            }

            public void Draw(RenderTarget target, UiContext context)
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
        private readonly Library<IconAtom> _atoms;
        private readonly EnumMap<ComponentType, DesignedComponentIconConfig> _configs;
        private readonly Dictionary<Type, Func<object, IEnumerable<IconLayer>>> _definitionMap;
        private readonly UiElementFactory _uiElementFactory;
        private readonly RenderShader _shader;

        private readonly RenderTexture _rasterTexture = new(new(64, 64));

        public IconFactory(
            BannerViewFactory bannerViewFactory,
            Library<IconAtom> atoms,
            EnumMap<ComponentType, DesignedComponentIconConfig> configs,
            UiElementFactory uiElementFactory)
        {
            _bannerViewFactory = bannerViewFactory;
            _atoms = atoms;
            _configs = configs;
            _definitionMap = new()
            {
                { typeof(BaseComponent),  GetAtomicDefinition },
                { typeof(BaseMaterial), GetAtomicDefinition },
                { typeof(BattalionTemplate), GetDesignedComponentDefinition },
                { typeof(Design), GetDesignDefinition },
                { typeof(DesignedComponent), GetDesignedComponentDefinition },
                { typeof(DivisionTemplate), GetDesignedComponentDefinition },
                { typeof(Faction), GetBannerDefinition },
                { typeof(Fleet), GetFormationDefinition },
                { typeof(FleetDriver), GetDriverDefinition },
                { typeof(Recipe), GetRecipeDefinition },
                { typeof(Structure), GetAtomicDefinition },
                { typeof(Unit), GetDesignedComponentDefinition },
                { typeof(UnitGrouping), GetUnitGroupingDefinition }
            };
            _uiElementFactory = uiElementFactory;
            _shader = _uiElementFactory.GetShader("shader-default");
        }

        public Icon Create(Class @class, IElementController controller, object @object)
        {
            return 
                new(
                    @class, 
                    controller, 
                    Rasterize(new IconConfig(GetDefinition(@object), _uiElementFactory, _shader), new IconCamera()),
                    _shader,
                    64);
        }

        public IEnumerable<IconLayer> GetDefinition(object @object)
        {
            return _definitionMap[@object.GetType()](@object);
        }

        private IEnumerable<IconLayer> GetAtomicDefinition(object @object)
        {
            var key = @object as IKeyed;
            yield return _atoms[key!.Key].ToDefinition();
        }

        private IEnumerable<IconLayer> GetBannerDefinition(object @object)
        {
            return _bannerViewFactory.Create(((Faction)@object).Banner);
        }

        private IEnumerable<IconLayer> GetDesignDefinition(object @object)
        {
            var design = (Design)@object;
            return GetDefinition(design.Components.First());
        }

        private IEnumerable<IconLayer> GetDesignedComponentDefinition(object @object)
        {
            var component = (DesignedComponent)@object;
            return _configs[component.Slot.Type].CreateDefinition(
                component, new(Color4.White, Color4.Black, Color4.Red), this);
        }

        private IEnumerable<IconLayer> GetDriverDefinition(object @object)
        {
            var driver = (IFormationDriver)@object;
            return GetDefinition(driver.Formation);
        }

        private IEnumerable<IconLayer> GetFormationDefinition(object @object)
        {
            return GetBannerDefinition(((IFormation)@object).Faction);
        }

        private IEnumerable<IconLayer> GetRecipeDefinition(object @object)
        {
            var recipe = (Recipe)@object;
            return GetDefinition(recipe.Transformation.First(x => x.Value > 0).Key);
        }

        private IEnumerable<IconLayer> GetUnitGroupingDefinition(object @object)
        {
            var grouping = (UnitGrouping)@object;
            return GetDefinition(grouping.Unit);
        }

        private Texture Rasterize(IRenderable renderable, ICamera camera)
        {
            _rasterTexture.Clear();
            _rasterTexture.PushModelMatrix(Matrix4.Identity);
            _rasterTexture.PushViewMatrix(camera.GetViewMatrix());
            _rasterTexture.PushProjection(camera.GetProjection());
            renderable.Draw(_rasterTexture, new());
            _rasterTexture.PopProjectionMatrix();
            _rasterTexture.PopViewMatrix();
            _rasterTexture.PopModelMatrix();
            _rasterTexture.Display();
            return _rasterTexture.CopyTexture();
        }
    }
}
