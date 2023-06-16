using Cardamom.Ui;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.DesignPanes
{
    public class MilitaryOrganizationPane : DesignPane
    {
        public MilitaryOrganizationPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
        : base(
                uiElementFactory,
                iconFactory,
                new ComponentType[]
                {
                    ComponentType.DivisionTemplate,
                    ComponentType.BattalionTemplate,
                    ComponentType.Infantry
                })
            { }
    }
}
