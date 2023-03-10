using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Panes.DesignPanes;

namespace SpaceOpera.Controller.Panes.DesignPanes
{
    public class DesignerComponentCellController 
        : ClassedUiElementController<UiContainer>, IOptionController<DesignSlot>
    {
        public DesignSlot Key {get;}

        private DesignerComponentCell? _cell;
        private IComponent? _value;

        public DesignerComponentCellController(DesignSlot slot)
        {
            Key = slot;
        }

        public override void Bind(object @object)
        {
            base.Bind(@object);
            _cell = @object as DesignerComponentCell;
        }

        public override void Unbind()
        {
            base.Unbind();
            _cell = null;
        }

        public IComponent? GetValue()
        {
            return _value;
        }

        public void SetValue(IComponent? value)
        {
            _value = value;
            _cell?.SetComponent(value);
        }

        public void SetSelected(bool selected)
        {
            SetToggle(selected);
        }

        public override bool HandleMouseButtonClicked(MouseButtonClickEventArgs e)
        {
            Clicked?.Invoke(this, e);
            return true;
        }

        public override bool HandleMouseEntered()
        {
            SetHover(true);
            return true;
        }

        public override bool HandleMouseLeft()
        {
            SetHover(false);
            return true;
        }

        public override bool HandleFocusEntered()
        {
            SetFocus(true);
            return true;
        }

        public override bool HandleFocusLeft()
        {
            SetFocus(false);
            return true;
        }

        private void HandleClick(object? sender, MouseButtonClickEventArgs e)
        {
            Clicked?.Invoke(this, e);
        }
    }
}
