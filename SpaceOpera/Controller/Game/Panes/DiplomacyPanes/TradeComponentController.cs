using Cardamom.Trackers;
using Cardamom.Utils.Suppliers;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Diplomacy;
using SpaceOpera.View.Forms;
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
            var materials = 
                _world.GetRecipesFor(_faction)
                    .SelectMany(x => x.Transformation)
                    .Where(x => x.Value > 0)
                    .Select(x => x.Key);
            var materialsInput = 
                new FormLayout.Builder()
                    .SetTitle("Trade Proposal")
                    .AddHidden("holding", holding)
                    .AddMultiCount()
                        .SetId("materials")
                        .SetOptionNameFn(x => ((IMaterial)x).Name);
            foreach (var material in materials)
            {
                materialsInput.AddOption(material);
            }
            var promise = new Promise<FormValue>();
            promise.Canceled += HandleFormCanceled;
            promise.Finished += HandleFormComplete;
            PopupCreated?.Invoke(this, new(materialsInput.Complete().Build(), promise));
        }

        private void HandleFormCanceled(object? sender, EventArgs e)
        {
            var promise = (Promise<FormValue>)sender!;
            promise.Canceled -= HandleFormCanceled;
            promise.Finished -= HandleFormComplete;
        }

        private void HandleFormComplete(object? sender, EventArgs e)
        {
            var promise = (Promise<FormValue>)sender!;
            promise.Canceled -= HandleFormCanceled;
            promise.Finished -= HandleFormComplete;

            var left = (StellarBodyHolding)promise.Get()["holding"]!;
            // TODO: Transfer to other faction's holding.
            var right = left;
            var materials =
                ((MultiCount<object>)promise.Get()["materials"]!).ToMultiQuantity(x => (IMaterial)x.Key, x => x.Value);
            Added?.Invoke(this, new TradeAgreement(new(left, right, materials)));
        }
    }
}
