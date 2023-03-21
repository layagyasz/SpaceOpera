using Cardamom;
using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.FactionViews;

namespace SpaceOpera.View.Icons
{
    public class IconFactory
    {
        private readonly BannerViewFactory _bannerViewFactory;
        private readonly Library<IconAtom> _atoms;
        private readonly EnumMap<ComponentType, DesignedComponentIconConfig> _configs;
        private readonly Dictionary<Type, Func<object, IEnumerable<IconLayer.Definition>>> _definitionMap;
        private readonly UiElementFactory _uiElementFactory;
        private readonly RenderShader _shader;

        private readonly BannerColorSet _factionColor;

        private IconFactory(
            BannerViewFactory bannerViewFactory,
            Library<IconAtom> atoms,
            EnumMap<ComponentType, DesignedComponentIconConfig> configs,
            UiElementFactory uiElementFactory,
            BannerColorSet factionColor)
        {
            _bannerViewFactory = bannerViewFactory;
            _atoms = atoms;
            _configs = configs;
            _definitionMap = new()
            {
                { typeof(BaseComponent),  GetAtomicDefinition },
                { typeof(BaseMaterial), GetAtomicDefinition },
                { typeof(BattalionTemplate), GetDesignedComponentDefinition },
                { typeof(DesignedComponent), GetDesignedComponentDefinition },
                { typeof(DivisionTemplate), GetDesignedComponentDefinition },
                { typeof(Faction), GetBannerDefinition },
                { typeof(Fleet), GetFormationDefinition },
                { typeof(Unit), GetDesignedComponentDefinition }
            };
            _uiElementFactory = uiElementFactory;
            _shader = _uiElementFactory.GetShader("shader-default");
            _factionColor = factionColor;
        }

        public IconFactory(
            BannerViewFactory bannerViewFactory,
            Library<IconAtom> atoms,
            EnumMap<ComponentType, DesignedComponentIconConfig> configs,
            UiElementFactory uiElementFactory)
            : this(bannerViewFactory, atoms, configs, uiElementFactory, BannerColorSet.Default) { }

        public Icon Create(Class @class, IElementController controller, object @object)
        {
            return new(@class, controller, GetDefinition(@object).Select(x => x.Create(_shader, _uiElementFactory)));
        }

        public IEnumerable<IconLayer.Definition> GetDefinition(object @object)
        {
            return _definitionMap[@object.GetType()](@object);
        }

        public IconFactory ForFaction(Faction faction)
        {
            return new(
                _bannerViewFactory, _atoms, _configs, _uiElementFactory, _bannerViewFactory.Get(faction.Banner));
        }

        private IEnumerable<IconLayer.Definition> GetAtomicDefinition(object @object)
        {
            var key = @object as IKeyed;
            yield return _atoms[key!.Key].ToDefinition();
        }

        private IEnumerable<IconLayer.Definition> GetBannerDefinition(object @object)
        {
            return _bannerViewFactory.Create(((Faction)@object).Banner);
        }

        private IEnumerable<IconLayer.Definition> GetDesignedComponentDefinition(object @object)
        {
            var component = @object as DesignedComponent;
            return _configs[component!.Slot.Type].CreateDefinition(component, _factionColor, this);
        }

        private IEnumerable<IconLayer.Definition> GetFormationDefinition(object @object)
        {
            return GetBannerDefinition(((IFormation)@object).Faction);
        }
    }
}
