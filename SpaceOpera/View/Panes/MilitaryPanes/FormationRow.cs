using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.DesignPanes
{
    public class FormationRow : UiSerialContainer, IKeyedUiElement<IFormation>, IActionRow
    {
        private static readonly string s_FormationRowClassName = "military-pane-formation-row";
        private static readonly string s_FormationRowIconClassName = "military-pane-formation-row-icon";
        private static readonly string s_FormationRowTextClassName = "military-pane-formation-row-text";

        public EventHandler<EventArgs>? Refreshed { get; set; }

        public IFormation Key { get; }

        private FormationRow(
            Class @class, IFormation formation, Icon icon, IUiElement text)
            : base(@class, new ActionRowController<IFormation>(formation), Orientation.Horizontal)
        {
            Key = formation;
            Add(icon);
            Add(text);
        }

        public IEnumerable<IUiElement> GetActions()
        {
            return Enumerable.Empty<IUiElement>();
        }

        public static FormationRow Create(
            IFormation formation, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return new(
                uiElementFactory.GetClass(s_FormationRowClassName),
                formation,
                iconFactory.Create(
                    uiElementFactory.GetClass(s_FormationRowIconClassName),
                    new ButtonController(),
                    formation),
                uiElementFactory.CreateTextButton(s_FormationRowTextClassName, formation.Name).Item1);
        }

        public void Refresh()
        {
            Refreshed?.Invoke(this, EventArgs.Empty);
        }
    }
}
