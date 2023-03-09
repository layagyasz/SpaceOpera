using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Panes.DesignPanes;

namespace SpaceOpera.Controller.Panes.DesignPanes
{
    public class DesignerComponentCellController : ButtonController, IFormElementController<DesignSlot, IComponent>
    {
        public EventHandler<ValueChangedEventArgs<DesignSlot, IComponent>>? ValueChanged { get; set; }

        public DesignSlot Key { get; }

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
    }
}
