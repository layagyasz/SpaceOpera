using Cardamom.Trackers;
using Cardamom.Utils.Suppliers;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Diplomacy;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Forms;
using SpaceOpera.View.Game.Panes.DiplomacyPanes;

namespace SpaceOpera.Controller.Game.Panes.DiplomacyPanes
{
    public class TradeComponentController : IDiplomacyOptionController
    {
        public EventHandler<IDiplomaticAgreementSection>? Added { get; set; }
        public EventHandler<PopupEventArgs>? PopupCreated { get; set; }

        private readonly World _world;
        private readonly Faction _left;
        private readonly Faction _right;

        private TradeComponent? _component;
        private IAdderController<StellarBody>? _options;

        public TradeComponentController(World world, Faction left, Faction right)
        {
            _world = world;
            _left = left;
            _right = right;
        }

        public void Bind(object @object)
        {
            _component = (TradeComponent)@object;
            _options = (IAdderController<StellarBody>)_component.Options.ComponentController;
            _options.Added += HandleAdd;
        }

        public void Unbind()
        {
            _options!.Added -= HandleAdd;
            _options = null;
            _component = null;
        }

        private void HandleAdd(object? sender, StellarBody stellarBody)
        {
            var materials = 
                _world.GetRecipesFor(_left)
                    .SelectMany(x => x.Transformation)
                    .Where(x => x.Value > 0)
                    .Select(x => x.Key);
            var materialsInput = 
                new FormLayout.Builder()
                    .SetTitle("Trade Proposal")
                    .AddHidden("stellarBody", stellarBody)
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

            var stellarBody = (StellarBody)promise.Get()["stellarBody"]!;
            var leftHolding = _world.Economy.GetHolding(_left, stellarBody)!;
            var rightHolding = _world.Economy.GetHolding(_right, stellarBody)!;
            var materials =
                ((MultiCount<object>)promise.Get()["materials"]!).ToMultiQuantity(x => (IMaterial)x.Key, x => x.Value);
            Added?.Invoke(this, new TradeAgreement(new(leftHolding, rightHolding, materials)));
        }
    }
}
