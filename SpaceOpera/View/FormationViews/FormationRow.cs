using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.FormationsViews;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.FormationViews
{
    public class FormationRow : DynamicUiCompoundComponent, IActionRow
    {
        private static readonly string s_FormationLayerRowClassName = "formation-layer-row";
        private static readonly string s_FormationLayerRowIconClassName = "formation-layer-row-icon";
        private static readonly string s_FormationLayerRowTextClassName = "formation-layer-row-text";
        private static readonly string s_FormationLayerRowBattleIconClassName = "formation-layer-row-battle-icon";

        private readonly List<FormationDriver> _drivers = new();
        private readonly IUiElement _battle;

        private FormationRow(Class @class, Icon icon, IUiElement text, IUiElement battle)
            : base(
                  new FormationRowController(), 
                  new UiSerialContainer(@class, new ButtonController(), UiSerialContainer.Orientation.Horizontal))
        {
            _battle = battle;

            Add(icon);
            Add(text);
            Add(battle);

            Refresh();
        }

        public void Add(FormationDriver driver)
        {
            _drivers.Add(driver);
        }

        public void Remove(FormationDriver driver)
        {
            _drivers.Remove(driver);
        }

        public IEnumerable<IUiElement> GetActions()
        {
            yield return _battle;
        }

        public IEnumerable<FormationDriver> GetDrivers()
        {
            return _drivers;
        }

        public override void Refresh()
        {
            _battle.Visible = _drivers.Any(x => x.Formation.InCombat);
            base.Refresh();
        }

        public static FormationRow Create(
            object key, string name, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return new(
                uiElementFactory.GetClass(s_FormationLayerRowClassName),
                iconFactory.Create(
                    uiElementFactory.GetClass(s_FormationLayerRowIconClassName),
                    new InlayController(),
                    key),
                new TextUiElement(
                    uiElementFactory.GetClass(s_FormationLayerRowTextClassName), new InlayController(), name),
                new SimpleUiElement(
                    uiElementFactory.GetClass(s_FormationLayerRowBattleIconClassName),
                    new ActionButtonController(ActionId.Battle)));
        }
    }
}
