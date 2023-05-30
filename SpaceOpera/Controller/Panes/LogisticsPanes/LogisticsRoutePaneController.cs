using Cardamom.Trackers;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Panes.LogisticsPanes;

namespace SpaceOpera.Controller.Panes.LogisticsPanes
{
    public class LogisticsRoutePaneController : GamePaneController
    {
        public override void Bind(object @object)
        {
            base.Bind(@object);
            var pane = (LogisticsRoutePane)_pane!;
            pane.Populated += HandlePopulated;
            BindInterceptor((IInterceptorController)pane.LeftAnchor.Controller);
            BindInterceptor((IInterceptorController)pane.RightAnchor.Controller);
            BindInterceptor((IInterceptorController)pane.Fleets.Adder.Controller);
        }

        public override void Unbind()
        {
            var pane = (LogisticsRoutePane)_pane!;
            pane.Populated -= HandlePopulated;
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

            var leftAnchor = (InterceptorInputController<StellarBodyRegion>)pane.LeftAnchor.Controller;
            leftAnchor.SetValue(null);
            var rightAnchor = (InterceptorInputController<StellarBodyRegion>)pane.RightAnchor.Controller;
            rightAnchor.SetValue(null);

            var leftMaterials = (ManualNumericInputTableController<IMaterial>)pane.LeftMaterials.ComponentController;
            leftMaterials.SetValue(pane.GetSeedRoute()?.LeftMaterials.ToMultiCount(x => x.Key, x => (int)x.Value));
            var rightMaterials = (ManualNumericInputTableController<IMaterial>)pane.RightMaterials.ComponentController;
            rightMaterials.SetValue(pane.GetSeedRoute()?.RightMaterials.ToMultiCount(x => x.Key, x => (int)x.Value));

            var fleets = (InterceptorMultiSelectController<FleetDriver>)pane.Fleets.ComponentController;
            fleets.SetValue(Enumerable.Empty<FleetDriver>());
        }
    }
}
