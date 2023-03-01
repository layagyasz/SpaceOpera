using SpaceOpera.View.Overlay;

namespace SpaceOpera.Controller.Overlay
{
    public class EmpireOverlayController : IOverlayController
    {
        public EventHandler<ElementEventArgs<OverlayButtonId>>? ButtonClicked { get; set; }

        private EmpireOverlay? _overlay;

        public void Bind(object @object)
        {
            _overlay = @object as EmpireOverlay;
            foreach (var controller in _overlay!.GetButtons().Select(x => x.Controller).Cast<IOverlayController>())
            {
                controller.ButtonClicked += HandleClick;
            }
        }

        public void Unbind()
        {
            foreach (var controller in _overlay!.GetButtons().Select(x => x.Controller).Cast<IOverlayController>())
            {
                controller.ButtonClicked -= HandleClick;
            }
            _overlay = null;
        }

        private void HandleClick(object? sender, ElementEventArgs<OverlayButtonId> e)
        {
            ButtonClicked?.Invoke(this, e);
        }
    }
}
