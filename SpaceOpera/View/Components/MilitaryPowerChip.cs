using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Components.Dynamics;

namespace SpaceOpera.View.Components
{
    public class MilitaryPowerChip
    {
        public static IUiElement Create(
            Generator<float> power, ChipSetStyles.ChipStyle style, UiElementFactory uiElementFactory)
        {
            return new DynamicUiSerialContainer(
                uiElementFactory.GetClass(style.Container!),
                new InlayController(),
                UiSerialContainer.Orientation.Horizontal)
            {
                new SimpleUiElement(uiElementFactory.GetClass(style.Icon!), new InlayController()),
                new DynamicTextUiElement(
                    uiElementFactory.GetClass(style.Text!), new InlayController(), () => Format(power()))
            };
        }

        private static string Format(float value)
        {
            if (value > 10000f)
            {
                return string.Format("{0:N0}K", 0.001f * value);
            }
            return value.ToString("N0");
        }
    }
}
