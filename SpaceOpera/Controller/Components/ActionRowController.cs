using Cardamom.Ui;
using Cardamom.Ui.Controller;
using SpaceOpera.View;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class ActionRowController<T> : IController, IActionController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        public T Key { get; }
        public ActionId ClickAction { get; }

        private IActionRow? _row;

        public ActionRowController(T key, ActionId clickAction)
        {
            Key = key;
            ClickAction = clickAction;
        }

        public void Bind(object @object)
        {
            _row = (IActionRow)@object;
            _row.Controller.Clicked += HandleClick;
            _row.ActionAdded += HandleActionAdded;
            _row.ActionRemoved += HandleActionRemoved;
            foreach (var actionController in _row!.GetActions().Select(x => x.Controller).Cast<IActionController>())
            {
                actionController.Interacted += HandleInteraction;
            }
        }

        public void Unbind()
        {
            foreach (var actionController in _row!.GetActions().Select(x => x.Controller).Cast<IActionController>())
            {
                actionController.Interacted -= HandleInteraction;
            }
            _row.Controller.Clicked -= HandleClick;
            _row.ActionAdded -= HandleActionAdded;
            _row.ActionRemoved -= HandleActionRemoved;
            _row = null;
        }

        private void HandleActionAdded(object? sender, ElementEventArgs e)
        {
            var controller = (IActionController)((IUiElement)e.Element).Controller;
            controller.Interacted += HandleInteraction;
        }

        private void HandleActionRemoved(object? sender, ElementEventArgs e)
        {
            var controller = (IActionController)((IUiElement)e.Element).Controller;
            controller.Interacted -= HandleInteraction;
        }

        private void HandleClick(object? sender, MouseButtonClickEventArgs e)
        {
            Interacted?.Invoke(this, UiInteractionEventArgs.Create(Key!, ClickAction));
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e.WithObject(Key!));
        }
    }
}
