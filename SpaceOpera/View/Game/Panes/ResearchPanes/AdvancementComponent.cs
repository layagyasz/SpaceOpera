using Cardamom.Ui;
using SpaceOpera.Core.Advancement;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.ResearchPanes
{
    public static class AdvancementComponent
    {
        public class Style
        {
            public string? Container { get; set; }
            public string? Icon { get; set; }
            public string? Info { get; set; }
            public string? Text { get; set; }
            public string? ProgressText { get; set; }
            public string? Progress { get; set; }
        }

        public static IUiElement Create(
            IAdvancement advancement,
            FactionAdvancementManager advancementManager,
            Style style,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory)
        {

        }
    }
}
