using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;

namespace SpaceOpera.View.Components.Dynamics
{
    public class DynamicUiGroup : UiGroup, IDynamic
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        public DynamicUiGroup(IController controller) : base(controller) { }

        public virtual void Refresh()
        {
            foreach (var element in _elements)
            {
                if (element is IDynamic dynamic)
                {
                    dynamic.Refresh();
                }
            }
            Refreshed?.Invoke(this, EventArgs.Empty);
        }
    }
}
