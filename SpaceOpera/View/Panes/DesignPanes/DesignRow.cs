using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Components;

namespace SpaceOpera.View.Panes.DesignPanes
{
    public class DesignRow : TextUiElement, IKeyedUiElement<Design>
    {
        private static readonly string s_ComponentRowClassName = "design-pane-component-row";

        public Design Key { get; }

        private DesignRow(Class @class, Design design)
            : base(@class, new ButtonController(), design.Name)
        {
            Key = design;
        }

        public static DesignRow Create(Design design, UiElementFactory uiElementFactory)
        {
            return new(uiElementFactory.GetClass(s_ComponentRowClassName), design);
        }

        public void Refresh() { }
    }
}
