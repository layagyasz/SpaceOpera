using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.View;
using SpaceOpera.View.FormationViews;

namespace SpaceOpera.Controller.FormationsViews
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
            var controller = (TableController)element.Controller;
            controller.ElementClicked += HandleElementClicked;
        }

        private void UnbindElement(object @object)
        {
            var element = (FormationList)@object;
            var controller = (TableController)element.Controller;
            controller.ElementClicked -= HandleElementClicked;
        }

        private void HandleElementAdded(object? sender, ElementEventArgs e)
        {
            BindElement(e.Element);
        }

        private void HandleElementRemoved(object? sender, ElementEventArgs e)
        {
            UnbindElement(e);
        }

        private void HandleElementClicked(object? sender, ElementClickedEventArgs e)
        {
            var row = (FormationRow)((ButtonController)e.Element).GetElement();
            Interacted?.Invoke(this, UiInteractionEventArgs.Create(row.GetFormations(), ActionId.Select));
        }
    }
}
