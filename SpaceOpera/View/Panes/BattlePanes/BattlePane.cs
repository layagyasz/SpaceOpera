using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Controller.Panes;
using SpaceOpera.Core.Military.Battles;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.BattlePanes
{
    public class BattlePane : SimpleGamePane
    {
        private static readonly string s_ClassName = "battle-pane";
        private static readonly string s_TitleClassName = "battle-pane-title";
        private static readonly string s_CloseClass = "battle-pane-close";

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;
        private readonly Configuration _configuration = new();

        private Battle? _battle;

        class Configuration
        {
            private BattleReport? _report;

            public void SetReport(BattleReport? report)
            {
                _report = report;
            }
        }

        public BattlePane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new GamePaneController(),
                  uiElementFactory.GetClass(s_ClassName),
                  new TextUiElement(uiElementFactory.GetClass(s_TitleClassName), new ButtonController(), string.Empty),
                  uiElementFactory.CreateSimpleButton(s_CloseClass).Item1)
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
        }

        public override void Populate(params object?[] args)
        {
            _battle = args[0] as Battle;
            SetTitle($"Battle in {_battle?.Location?.Name}");
            Populated?.Invoke(this, EventArgs.Empty);
            Refresh();
        }

        public override void Refresh()
        {
            _configuration.SetReport(_battle?.GetReport());
            base.Refresh();
        }
    }
}
