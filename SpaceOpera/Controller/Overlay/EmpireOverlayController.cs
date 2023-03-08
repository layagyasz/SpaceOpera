using Cardamom.Ui;
using SpaceOpera.Controller.Components;
using SpaceOpera.View.Overlay;

namespace SpaceOpera.Controller.Overlay
{
    public class EmpireOverlayController : IActionController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        private EmpireOverlay? _overlay;

        public void Bind(object @object)
        {
            _overlay = @object as EmpireOverlay;
            foreach (var controller 
                in _overlay!.Cast<IUiElement>().Select(x => x.Controller).Cast<IActionController>())
            {
                controller.Interacted += HandleInteraction;
            }
        }

        public void Unbind()
        {
            foreach (var controller 
                in _overlay!.Cast<IUiElement>().Select(x => x.Controller).Cast<IActionController>())
            {
                controller.Interacted -= HandleInteraction;
            }
            _overlay = null;
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }
    }
}
