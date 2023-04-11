using Cardamom.Ui.Controller;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class ActionRowController<T> : IController, IActionController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        public T Key { get; }

        private IActionRow? _row;

        public ActionRowController(T key)
        {
            Key = key;
        }

        public void Bind(object @object)
        {
            _row = @object as IActionRow;
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
            _row = null;
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e.WithObject(Key!));
        }
    }
}
