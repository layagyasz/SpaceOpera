using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.DesignPanes
{
    public class DesignerComponentOption : UiSerialContainer
    {
        private static readonly string s_ComponentRow = "designer-pane-component-option-row";
        private static readonly string s_ComponentRowIcon = "designer-pane-component-option-row-icon";
        private static readonly string s_ComponentRowText = "designer-pane-component-option-row-text";

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
                uiElementFactory.GetClass(s_ComponentRow),
                component,
                iconFactory.Create(
                    uiElementFactory.GetClass(s_ComponentRowIcon),
                    new InlayController(),
                    component),
                new TextUiElement(
                    uiElementFactory.GetClass(s_ComponentRowText), new InlayController(), component.Name));
        }
    }
}
