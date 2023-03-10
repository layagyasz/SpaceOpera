using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.DesignPanes
{
    public class DesignerComponentOption : UiSerialContainer
    {
        private static readonly string s_ComponentRowClassName = "designer-pane-component-option-row";
        private static readonly string s_ComponentRowIconClassName = "designer-pane-component-option-row-icon";
        private static readonly string s_ComponentRowTextClassName = "designer-pane-component-option-row-text";

        private DesignerComponentOption(Class @class, IComponent component, Icon icon, IUiElement text)
            : base(@class, new OptionElementController<IComponent>(component), Orientation.Horizontal)
        {
            Add(icon);
            Add(text);
        }

        public static DesignerComponentOption Create(
            IComponent component, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return new(
                uiElementFactory.GetClass(s_ComponentRowClassName),
                component,
                iconFactory.Create(
                    uiElementFactory.GetClass(s_ComponentRowIconClassName),
                    new InlayController(),
                    component),
                new TextUiElement(
                    uiElementFactory.GetClass(s_ComponentRowTextClassName), new InlayController(), component.Name));
        }
    }
}
