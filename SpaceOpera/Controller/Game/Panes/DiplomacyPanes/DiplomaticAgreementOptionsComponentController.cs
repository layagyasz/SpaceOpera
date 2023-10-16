using Cardamom.Ui;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.Controller.Game.Panes.DiplomacyPanes
{
    public class DiplomaticAgreementOptionsComponentController : 
        AdderComponentController<IDiplomaticAgreementSection>, IPopupController
    {
        public EventHandler<PopupEventArgs>? PopupCreated { get; set; }

        protected override void BindElement(IUiElement element)
        {
            base.BindElement(element);
            if (element is IUiComponent row)
            {
                var controller = (IPopupController)row.ComponentController;
                controller!.PopupCreated += HandlePopup;
            }
        }

        protected override void UnbindElement(IUiElement element)
        {
            base.UnbindElement(element);
            if (element is IUiComponent row)
            {
                var controller = (IPopupController)row.ComponentController;
                controller!.PopupCreated += HandlePopup;
            }
        }

        private void HandlePopup(object? sender, PopupEventArgs e)
        {
            PopupCreated?.Invoke(this, e);
        }
    }
}
