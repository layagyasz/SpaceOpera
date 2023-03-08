using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.DesignPanes
{
    public class DesignRow : UiSerialContainer, IKeyedUiElement<Design>, IActionRow
    {
        private static readonly string s_ComponentRowClassName = "design-pane-component-row";
        private static readonly string s_ComponentRowIconClassName = "design-pane-component-row-icon";
        private static readonly string s_ComponentRowTextClassName = "design-pane-component-row-text";
        private static readonly string s_ComponentRowActionContainer = "design-pane-component-row-action-container";
        private static readonly string s_ComponentRowActionEdit = "design-pane-component-row-action-edit";

        public Design Key { get; }

        private readonly List<IUiElement> _actions;

        private DesignRow(Class @class, Design design, Icon icon, IUiElement text, params IUiElement[] actions)
            : base(@class, new ActionRowController<Design>(design), Orientation.Horizontal)
        {
            Key = design;
            Add(icon);
            Add(text);
            foreach (var action in actions)
            {
                Add(action);
            }
            _actions = actions.ToList();
        }

        public IEnumerable<IUiElement> GetActions()
        {
            return _actions;
        }

        public static DesignRow Create(Design design, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return new(
                uiElementFactory.GetClass(s_ComponentRowClassName),
                design,
                iconFactory.Create(
                    uiElementFactory.GetClass(s_ComponentRowIconClassName), 
                    new ButtonController(),
                    design.Components.First()),
                uiElementFactory.CreateTextButton(s_ComponentRowTextClassName, design.Name).Item1,
                new UiWrapper(
                    uiElementFactory.GetClass(s_ComponentRowActionContainer), 
                    new SimpleUiElement(
                        uiElementFactory.GetClass(s_ComponentRowActionEdit), 
                        new ActionButtonController(ActionId.Edit))));
        }

        public void Refresh() { }
    }
}
