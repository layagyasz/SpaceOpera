using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;

namespace SpaceOpera.View.Components
{
    public class DynamicUiContainer : UiContainer, IDynamic
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        public DynamicUiContainer(Class @class, IElementController controller) 
            : base(@class, controller) { }

        public void Refresh()
        {
            foreach (var element in _elements.Values)
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
