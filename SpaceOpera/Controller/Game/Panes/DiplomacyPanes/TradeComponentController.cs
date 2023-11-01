using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Politics.Diplomacy;
using SpaceOpera.View.Game.Panes.DiplomacyPanes;

namespace SpaceOpera.Controller.Game.Panes.DiplomacyPanes
{
    public class TradeComponentController : IDiplomacyOptionController
    {
        public EventHandler<IDiplomaticAgreementSection>? Added { get; set; }
        public EventHandler<PopupEventArgs>? PopupCreated { get; set; }

        private TradeComponent? _component;
        private IAdderController<StellarBodyHolding>? _options;

        public void Bind(object @object)
        {
            _component = (TradeComponent)@object;
            _options = (IAdderController<StellarBodyHolding>)_component.Options.ComponentController;
            _options.Added += HandleAdd;
        }

        public void Unbind()
        {
            _options!.Added -= HandleAdd;
            _options = null;
            _component = null;
        }

        private void HandleAdd(object? sender, StellarBodyHolding holding)
        {
            Console.WriteLine(holding.Name);
        }
    }
}
