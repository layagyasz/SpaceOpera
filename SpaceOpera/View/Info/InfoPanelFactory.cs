using Cardamom.Ui;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Info
{
    public class InfoPanelFactory
    {
        public UiElementFactory UiElementFactory { get; }
        public IconFactory IconFactory { get; }

        public InfoPanelFactory(UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            UiElementFactory = uiElementFactory;
            IconFactory = iconFactory;
        }

        public InfoPanel CreateInfoPanelFor(
            object @object, 
            string @class,
            string headerRowClass,
            string headerClass,
            string headerIconClass,
            string headerTextClass,
            string rowClass,
            string rowHeadingClass,
            string rowValueClass,
            string materialCellClass,
            string materialIconClass,
            string materialTextClass)
        {
            var infoPanel = 
                new InfoPanel(
                    UiElementFactory.GetClass(@class), 
                    headerRowClass,
                    headerClass, 
                    headerIconClass,
                    headerTextClass,
                    rowClass, 
                    rowHeadingClass,
                    rowValueClass, 
                    materialCellClass, 
                    materialIconClass,
                    materialTextClass,
                    UiElementFactory,
                    IconFactory);
            Populate(infoPanel, @object);
            return infoPanel;
        }

        public static void Populate(InfoPanel InfoPanel, object Object)
        {
            if (Object is Recipe recipe)
            {
                new RecipeDescriber().Describe(recipe, InfoPanel);
            }
            else if (Object is Structure structure)
            {
                new StructureDescriber().Describe(structure, InfoPanel);
            }
            else if (Object is StarSystem starSystem)
            {
                new StarSystemDescriber().Describe(starSystem, InfoPanel);
            }
            else if (Object is IComponent component)
            {
                new ComponentDescriber(true).Describe(component, InfoPanel);
            }
            else if (Object is Design design)
            {
                new DesignDescriber().Describe(design, InfoPanel);
            }
        }
    }
}