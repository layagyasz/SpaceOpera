using Cardamom.Ui;
using SpaceOpera.View.Game.FormationViews;

namespace SpaceOpera.Controller.Game.FormationsViews
{
    public class FormationSubLayerController<T> : IActionController where T : notnull
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        private FormationSubLayer<T>? _layer;

        public void Bind(object @object)
        {
            _layer = (FormationSubLayer<T>)@object!;
            _layer.ElementAdded += HandleElementAdded;
            _layer.ElementRemoved += HandleElementRemoved;
            foreach (var element in _layer)
            {
                BindElement(element);
            }
        }

        public void Unbind()
        {
            foreach (var element in _layer!)
            {
                UnbindElement(element);
            }
            _layer.ElementAdded -= HandleElementAdded;
            _layer.ElementRemoved -= HandleElementRemoved;
            _layer = null;
        }

        private void BindElement(object @object)
        {
            var element = (FormationList)@object;
            var controller = (IActionController)element.ComponentController;
            controller.Interacted += HandleInteraction;
        }

        private void UnbindElement(object @object)
        {
            var element = (FormationList)@object;
            var controller = (IActionController)element.ComponentController;
            controller.Interacted -= HandleInteraction;
        }

        private void HandleElementAdded(object? sender, ElementEventArgs e)
        {
            BindElement(e.Element);
        }

        private void HandleElementRemoved(object? sender, ElementEventArgs e)
        {
            UnbindElement(e.Element);
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }
    }
}
