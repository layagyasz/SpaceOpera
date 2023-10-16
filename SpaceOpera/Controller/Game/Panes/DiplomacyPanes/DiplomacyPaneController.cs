using SpaceOpera.View.Game.Panes.DiplomacyPanes;

namespace SpaceOpera.Controller.Game.Panes.DiplomacyPanes
{
    public class DiplomacyPaneController : GamePaneController
    {
        DiplomacyComponentController? _diplomacy;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            _diplomacy = (DiplomacyComponentController)((DiplomacyPane)_pane!).Diplomacy.ComponentController;
            _diplomacy.PopupCreated += HandlePopup;
        }

        public override void Unbind()
        {
            base.Unbind();
            _diplomacy!.PopupCreated -= HandlePopup;
            _diplomacy = null;
        }

        private void HandlePopup(object? sender, PopupEventArgs e)
        {
            PopupCreated?.Invoke(this, e);
        }
    }
}
