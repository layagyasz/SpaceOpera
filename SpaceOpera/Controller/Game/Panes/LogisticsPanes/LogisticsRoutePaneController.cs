using Cardamom.Trackers;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Components.NumericInputs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Orders.Formations;
using SpaceOpera.View.Game.Panes.LogisticsPanes;

namespace SpaceOpera.Controller.Game.Panes.LogisticsPanes
{
    public class LogisticsRoutePaneController : GamePaneController
    {
        public override void Bind(object @object)
        {
            base.Bind(@object);
            var pane = (LogisticsRoutePane)_pane!;
            pane.Populated += HandlePopulated;
            pane.Submit.Controller.Clicked += HandleSubmit;
            BindInterceptor((IInterceptorController)pane.LeftAnchor.Controller);
            BindInterceptor((IInterceptorController)pane.RightAnchor.Controller);
            BindInterceptor((IInterceptorController)pane.Fleets.Adder.Controller);
        }

        public override void Unbind()
        {
            var pane = (LogisticsRoutePane)_pane!;
            pane.Populated -= HandlePopulated;
            pane.Submit.Controller.Clicked -= HandleSubmit;
            UnbindInterceptor((IInterceptorController)pane.LeftAnchor.Controller);
            UnbindInterceptor((IInterceptorController)pane.RightAnchor.Controller);
            UnbindInterceptor((IInterceptorController)pane.Fleets.Adder.Controller);
            base.Unbind();
        }

        private void BindInterceptor(IInterceptorController controller)
        {
            controller.InterceptorCreated += HandleInterceptorCreated;
            controller.InterceptorCancelled += HandleInterceptorCancelled;
        }

        private void UnbindInterceptor(IInterceptorController controller)
        {
            controller.InterceptorCreated -= HandleInterceptorCreated;
            controller.InterceptorCancelled -= HandleInterceptorCancelled;
        }

        private void HandleInterceptorCreated(object? sender, IInterceptor e)
        {
            InterceptorCreated?.Invoke(this, e);
        }

        private void HandleInterceptorCancelled(object? sender, IInterceptor e)
        {
            InterceptorCancelled?.Invoke(this, e);
        }

        private void HandlePopulated(object? sender, EventArgs e)
        {
            var pane = (LogisticsRoutePane)_pane!;

            var leftAnchor = (IFormFieldController<EconomicSubzoneHolding>)pane.LeftAnchor.Controller;
            leftAnchor.SetValue(null);
            var rightAnchor = (IFormFieldController<EconomicSubzoneHolding>)pane.RightAnchor.Controller;
            rightAnchor.SetValue(null);

            var leftMaterials = (ManualMultiCountInputController<IMaterial>)pane.LeftMaterials.ComponentController;
            leftMaterials.SetValue(pane.GetSeedRoute()?.LeftMaterials.ToMultiCount(x => x.Key, x => (int)x.Value));
            var rightMaterials = (ManualMultiCountInputController<IMaterial>)pane.RightMaterials.ComponentController;
            rightMaterials.SetValue(pane.GetSeedRoute()?.RightMaterials.ToMultiCount(x => x.Key, x => (int)x.Value));

            var fleets = (InterceptorMultiSelectController<FleetDriver>)pane.Fleets.ComponentController;
            fleets.SetValue(Enumerable.Empty<FleetDriver>());
        }

        private void HandleSubmit(object? sender, MouseButtonClickEventArgs e)
        {
            var pane = (LogisticsRoutePane)_pane!;

            var leftAnchor = (IFormFieldController<EconomicSubzoneHolding>)pane.LeftAnchor.Controller;
            var rightAnchor = (IFormFieldController<EconomicSubzoneHolding>)pane.RightAnchor.Controller;

            var leftMaterials = (ManualMultiCountInputController<IMaterial>)pane.LeftMaterials.ComponentController;
            var rightMaterials = (ManualMultiCountInputController<IMaterial>)pane.RightMaterials.ComponentController;

            var fleets = (InterceptorMultiSelectController<FleetDriver>)pane.Fleets.ComponentController;

            if (leftAnchor.GetValue() != null && rightAnchor.GetValue() != null)
            {
                OrderCreated?.Invoke(
                    this, 
                    new CreatePersistentRouteOrder(
                        pane.GetFaction(),
                        new PersistentRoute(
                            pane.GetFaction(), 
                            leftAnchor.GetValue()!, 
                            leftMaterials.GetValue().ToMultiQuantity(x => x.Key, x => x.Value),
                            rightAnchor.GetValue()!,
                            rightMaterials.GetValue().ToMultiQuantity(x => x.Key, x => x.Value), 
                            fleets.GetValue().Select(x => (Fleet)x.AtomicFormation).ToList())));
                Closed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
