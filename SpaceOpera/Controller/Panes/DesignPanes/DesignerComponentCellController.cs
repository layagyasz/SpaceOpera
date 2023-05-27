using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Panes.DesignPanes;

namespace SpaceOpera.Controller.Panes.DesignPanes
{
    public class DesignerComponentCellController 
        : ClassedUiElementController<ClassedUiElement>, IOptionController<DesignSlot>
    {
        public EventHandler<EventArgs>? Selected { get; set; }

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
            if (e.Button == MouseButton.Left)
            {
                Selected?.Invoke(this, EventArgs.Empty);
            }
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
            Focused?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public override bool HandleFocusLeft()
        {
            SetFocus(false);
            return true;
        }
    }
}
