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
        private static readonly string s_FormationLayerRow = "formation-layer-row";
        private static readonly string s_FormationLayerRowIcon = "formation-layer-row-icon";
        private static readonly string s_FormationLayerRowText = "formation-layer-row-text";
        private static readonly string s_FormationLayerRowNumber = "formation-layer-row-number";
        private static readonly string s_FormationLayerRowBattleIcon = "formation-layer-row-battle-icon";

        public int FormationCount => _drivers.Count;

        private readonly List<FormationDriver> _drivers = new();
        private TextUiElement _number;
        private readonly IUiElement _battle;

        private FormationRow(Class @class, Icon icon, IUiElement text, TextUiElement number, IUiElement battle)
            : base(
                  new FormationRowController(), 
                  new UiSerialContainer(@class, new ButtonController(), UiSerialContainer.Orientation.Horizontal))
        {
            _number = number;
            _battle = battle;

            Add(icon);
            Add(text);
            Add(number);
            Add(battle);

            Refresh();
        }

        public void Add(FormationDriver driver)
        {
            _drivers.Add(driver);
            UpdateNumber();
        }

        public void Remove(FormationDriver driver)
        {
            _drivers.Remove(driver);
            UpdateNumber();
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

        private void UpdateNumber()
        {
            if (FormationCount > 1)
            {
                _number.SetText(FormationCount.ToString());
            }
            else
            {
                _number.SetText(string.Empty);
            }
        }

        public static FormationRow Create(
            object key, string name, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return new(
                uiElementFactory.GetClass(s_FormationLayerRow),
                iconFactory.Create(
                    uiElementFactory.GetClass(s_FormationLayerRowIcon),
                    new InlayController(),
                    key),
                new TextUiElement(
                    uiElementFactory.GetClass(s_FormationLayerRowText), new InlayController(), name),
                new TextUiElement(
                    uiElementFactory.GetClass(s_FormationLayerRowNumber), new InlayController(), string.Empty),
                new SimpleUiElement(
                    uiElementFactory.GetClass(s_FormationLayerRowBattleIcon),
                    new ActionButtonController(ActionId.Battle)));
        }
    }
}
