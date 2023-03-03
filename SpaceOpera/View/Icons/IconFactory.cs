using Cardamom;
using Cardamom.Collections;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.Core.Designs;

namespace SpaceOpera.View.Icons
{
    public class IconFactory
    {
        private readonly Library<IconAtom> _atoms;
        private readonly Dictionary<Type, Func<object, IEnumerable<IIconLayerDefinition>>> _definitionMap;
        private readonly UiElementFactory _uiElementFactory;
        private readonly IconShaders _shaders;

        public IconFactory(Library<IconAtom> atoms, UiElementFactory uiElementFactory)
        {
            _atoms = atoms;
            _definitionMap = new()
            {
                { typeof(BaseComponent),  GetAtomicDefinition }
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

        private IEnumerable<IIconLayerDefinition> GetDefinition(object @object)
        {
            return _definitionMap[@object.GetType()](@object);
        }

        private IEnumerable<IIconLayerDefinition> GetAtomicDefinition(object @object)
        {
            var key = @object as IKeyed;
            yield return _atoms[key!.Key].ToDefinition();
        }
    }
}
