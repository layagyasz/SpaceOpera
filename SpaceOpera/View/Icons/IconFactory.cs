using Cardamom;
using Cardamom.Collections;
using Cardamom.Graphics;
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
        private readonly Dictionary<Type, Func<object, IEnumerable<IconLayer.Definition>>> _definitionMap;
        private readonly UiElementFactory _uiElementFactory;
        private readonly RenderShader _shader;

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
            _shader = _uiElementFactory.GetShader("shader-default");
        }

        public Icon Create(Class @class, IElementController controller, object @object)
        {
            return new(@class, controller, GetDefinition(@object).Select(x => x.Create(_shader, _uiElementFactory)));
        }

        public IEnumerable<IconLayer.Definition> GetDefinition(object @object)
        {
            return _definitionMap[@object.GetType()](@object);
        }

        private IEnumerable<IconLayer.Definition> GetAtomicDefinition(object @object)
        {
            var key = @object as IKeyed;
            yield return _atoms[key!.Key].ToDefinition();
        }

        private IEnumerable<IconLayer.Definition> GetDesignedComponentDefinition(object @object)
        {
            var component = @object as DesignedComponent;
            // TODO: Use faction's banner colors.
            return _configs[component!.Slot.Type].CreateDefinition(component, Color4.Black, this);
        }
    }
}
