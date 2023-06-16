using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Game.Panes.DesignPanes;

namespace SpaceOpera.Controller.Game.Panes.DesignPanes
{
    public class DesignerPaneController : GamePaneController
    {
        private RadioController<IComponent>? _componentTableController;
        private DesignerSegmentTableController? _segmentTableController;

        private DesignerComponentCellController? _activeCell;

        private Design? _design;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            var pane = (DesignerPane)_pane!;
            pane.Populated += HandlePopulated;
            _componentTableController = (RadioController<IComponent>)pane.ComponentOptionTable.ComponentController;
            _componentTableController.ValueChanged += HandleComponentSelected;
            _segmentTableController = (DesignerSegmentTableController)pane.SegmentTable.ComponentController;
            _segmentTableController!.CellSelected += HandleCellSelected;
            _segmentTableController!.ConfigurationChanged += HandleConfigurationChanged;
        }

        public override void Unbind()
        {
            var pane = (DesignerPane)_pane!;
            pane.Populated -= HandlePopulated;
            _componentTableController!.ValueChanged -= HandleComponentSelected;
            _componentTableController = null;
            _segmentTableController!.CellSelected -= HandleCellSelected;
            _segmentTableController!.ConfigurationChanged -= HandleConfigurationChanged;
            _segmentTableController = null;
            base.Unbind();
        }

        private void HandleCellSelected(object? sender, ValueEventArgs<IElementController> e)
        {
            _activeCell?.SetSelected(false);
            _activeCell = null;
            var newCell = e.Element as DesignerComponentCellController;

            var pane = (DesignerPane)_pane!;
            pane.SetSlot(newCell?.Key);
            if (newCell != null)
            {
                _componentTableController!.SetValue(newCell.GetValue()!);
            }

            _activeCell = newCell;
            _activeCell?.SetSelected(true);
        }

        private void HandleComponentSelected(object? sender, IComponent? e)
        {
            if (_activeCell != null)
            {
                _activeCell.SetValue(e);
                Update();
            }
        }

        private void HandleConfigurationChanged(
            object? sender, KeyValuePair<DesignerSegmentRow, SegmentConfiguration> e)
        {
            var pane = (DesignerPane)_pane!;
            if (_activeCell != null && e.Key.ComponentCells.Select(x => x.Controller).Contains(_activeCell))
            {
                _activeCell = null;
                pane.SetSlot(null);
            }
            pane.SetSegmentConfiguration(e.Key, e.Value);
            Update();
        }

        private void HandlePopulated(object? sender, EventArgs e)
        {
            Update();
        }

        private void Update()
        {
            var pane = (DesignerPane)_pane!;
            _design = pane.GetDesignBuilder().Build(new(pane.GetTemplate(), _segmentTableController!.GetSegments()));
            pane.SetInfo(_design);
        }
    }
}
