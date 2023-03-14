using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Panes.DesignPanes;

namespace SpaceOpera.Controller.Panes.DesignPanes
{
    public class DesignerPaneController : GamePaneController
    {
        private RadioController<IComponent>? _componentTableController;
        private DesignerSegmentTableController? _segmentTableController;

        private DesignerComponentCellController? _activeCell;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            var pane = (DesignerPane)_pane!;
            _componentTableController = (RadioController<IComponent>)pane.ComponentOptionTable.ComponentController;
            _componentTableController.ValueChanged += HandleComponentSelected;
            _segmentTableController = (DesignerSegmentTableController)pane.SegmentTable.ComponentController;
            _segmentTableController!.CellSelected += HandleCellSelected;
            _segmentTableController!.ConfigurationChanged += HandleConfigurationChanged;
        }

        public override void Unbind()
        {
            var pane = (DesignerPane)_pane!;
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

        private void HandleComponentSelected(object? sender, ValueChangedEventArgs<string, IComponent?> e)
        {
            _activeCell?.SetValue(e.Value);
        }

        private void HandleConfigurationChanged(
            object? sender, ValueChangedEventArgs<DesignerSegmentRow, SegmentConfiguration> e)
        {
            var pane = (DesignerPane)_pane!;
            if (_activeCell != null && e.Key.ComponentCells.Select(x => x.Controller).Contains(_activeCell))
            {
                _activeCell = null;
                pane.SetSlot(null);
            }
            pane.SetSegmentConfiguration(e.Key, e.Value);
        }
    }
}
