using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;
using SpaceOpera.View;
using SpaceOpera.View.Overlay.EmpireOverlays;

namespace SpaceOpera.Controller.Overlay
{
    public class EmpireOverlayController : IActionController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        private IUiContainer? _overlay;

        public void Bind(object @object)
        {
            _overlay = @object as GameOverlay;
            foreach (var element in _overlay!.Cast<IUiElement>())
            {
                BindController(element.Controller);
                if (element is UiCompoundComponent compound)
                {
                    BindController(compound.ComponentController);
                }
            }
        }

        public void Unbind()
        {
            foreach (var element in _overlay!.Cast<IUiElement>())
            {
                UnbindController(element.Controller);
                if (element is UiCompoundComponent compound)
                {
                    UnbindController(compound.ComponentController);
                }
            }
            _overlay = null;
        }

        private void BindController(IController controller)
        {
            if (controller is IActionController actionController)
            {
                actionController.Interacted += HandleInteraction;
            }
            if (controller is IFormElementController<string, ActionId> formController)
            {
                formController.ValueChanged += HandleSetInteraction;
            }
        }

        private void UnbindController(IController controller)
        {
            if (controller is IActionController actionController)
            {
                actionController.Interacted -= HandleInteraction;
            }
            if (controller is IFormElementController<string, ActionId> formController)
            {
                formController.ValueChanged -= HandleSetInteraction;
            }
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }

        private void HandleSetInteraction(object? sender, ValueChangedEventArgs<string, ActionId> e)
        {
            Interacted?.Invoke(this,  UiInteractionEventArgs.Create(Enumerable.Empty<object>(), e.Value));
        }
    }
}
