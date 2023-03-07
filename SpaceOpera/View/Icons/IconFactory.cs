using Cardamom;
using Cardamom.Collections;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.Core.Designs;

namespace SpaceOpera.View.Icons
{
    public class IconFactory
    {
        private readonly Library<IconAtom> _atoms;
        private readonly EnumMap<ComponentType, DesignedComponentIconConfig> _configs;
        private readonly Dictionary<Type, Func<object, IEnumerable<IIconLayerDefinition>>> _definitionMap;
        private readonly UiElementFactory _uiElementFactory;
        private readonly IconShaders _shaders;

        public IconFactory(
            Library<IconAtom> atoms, 
            EnumMap<ComponentType, DesignedComponentIconConfig> configs, 
            UiElementFactory uiElementFactory)
        {
            _atoms = atoms;
            _configs = configs;
            _definitionMap = new()
            {
                { typeof(BaseComponent),  GetAtomicDefinition },
                { typeof(DesignedComponent), GetDesignedComponentDefinition }
            };
            _uiElementFactory = uiElementFactory;
            _shaders =
                new(
                    _uiElementFactory.GetShader("shader-default-no-tex"), 
                    _uiElementFactory.GetShader("shader-default"));
        }

        public Icon Create(Class @class, IElementController controller, object @object)
        {
            return new(@class, controller, GetDefinition(@object).Select(x => x.Create(_shaders, _uiElementFactory)));
        }

        public IEnumerable<IIconLayerDefinition> GetDefinition(object @object)
        {
            return _definitionMap[@object.GetType()](@object);
        }

        private IEnumerable<IIconLayerDefinition> GetAtomicDefinition(object @object)
        {
            var key = @object as IKeyed;
            yield return _atoms[key!.Key].ToDefinition();
        }

        private IEnumerable<IIconLayerDefinition> GetDesignedComponentDefinition(object @object)
        {
            var component = @object as DesignedComponent;
            // TODO: Use faction's banner colors.
            return _configs[component!.Slot.Type].CreateDefinition(component, Color4.Black, this);
        }
    }
}
