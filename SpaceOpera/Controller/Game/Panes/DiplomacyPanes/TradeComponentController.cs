using SpaceOpera.Controller.Components;
using SpaceOpera.Core;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Diplomacy;
using SpaceOpera.View.Game.Panes.DiplomacyPanes;

namespace SpaceOpera.Controller.Game.Panes.DiplomacyPanes
{
    public class TradeComponentController : IDiplomacyOptionController
    {
        public EventHandler<IDiplomaticAgreementSection>? Added { get; set; }
        public EventHandler<PopupEventArgs>? PopupCreated { get; set; }

        private readonly World _world;
        private readonly Faction _faction;

        private TradeComponent? _component;
        private IAdderController<StellarBodyHolding>? _options;

        public TradeComponentController(World world, Faction faction)
        {
            _world = world;
            _faction = faction;
        }

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
