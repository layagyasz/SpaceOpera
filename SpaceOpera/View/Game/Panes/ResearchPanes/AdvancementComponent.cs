using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Advancement;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
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

        public static IUiContainer Create(
            IAdvancement advancement,
            FactionAdvancementManager advancementManager,
            IElementController controller,
            Style style,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory)
        {
            return new DynamicUiSerialContainer(
                uiElementFactory.GetClass(style.Container!),
                controller, 
                UiSerialContainer.Orientation.Horizontal)
            {
                iconFactory.Create(uiElementFactory.GetClass(style.Icon!), new InlayController(), advancement),
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(style.Info!),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Vertical)
                {
                    new TextUiElement(uiElementFactory.GetClass(style.Text!), new InlayController(), advancement.Name),
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(style.ProgressText!),
                        new InlayController(),
                        () => advancementManager.GetResearchProgress(advancement).ToString("N0")),
                    new PoolBar(
                        uiElementFactory.GetClass(style.Progress!),
                        new InlayController(), 
                        advancementManager.GetResearchProgress(advancement))
                }
            };
        }
    }
}
