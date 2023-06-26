using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Window;

namespace SpaceOpera.Controller.Components
{
    public class SliderInputKnobController : ClassedUiElementController<ClassedUiElement>
    {
        public override bool HandleMouseButtonClicked(MouseButtonClickEventArgs e)
        {
            Clicked?.Invoke(this, e);
            return false;
        }

        public override bool HandleMouseButtonDragged(MouseButtonDragEventArgs e)
        {
            MouseDragged?.Invoke(this, e);
            return true;
        }

        public override bool HandleMouseEntered()
        {
            SetHover(true);
            MouseEntered?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public override bool HandleMouseLeft()
        {
            SetHover(false);
            MouseLeft?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
