﻿using Cardamom.Trackers;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Economics;
using SpaceOpera.View.Panes.LogisticsPanes;

namespace SpaceOpera.Controller.Panes.LogisticsPanes
{
    public class LogisticsRoutePaneController : GamePaneController
    {
        public override void Bind(object @object)
        {
            base.Bind(@object);
            _pane!.Populated += HandlePopulated;
        }

        public override void Unbind()
        {
            _pane!.Populated -= HandlePopulated;
            base.Unbind();
        }

        private void HandlePopulated(object? sender, EventArgs e)
        {
            var pane = (LogisticsRoutePane)_pane!;
            var leftController = (ManualNumericInputTableController<IMaterial>)pane.LeftMaterials.ComponentController;
            leftController.SetValue(pane.GetSeedRoute()?.LeftMaterials.ToMultiCount(x => x.Key, x => (int)x.Value));
            var rightController = (ManualNumericInputTableController<IMaterial>)pane.LeftMaterials.ComponentController;
            rightController.SetValue(pane.GetSeedRoute()?.RightMaterials.ToMultiCount(x => x.Key, x => (int)x.Value));
        }
    }
}
