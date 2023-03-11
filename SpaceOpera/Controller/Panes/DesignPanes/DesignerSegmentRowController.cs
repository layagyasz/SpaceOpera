using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Panes.DesignPanes;

namespace SpaceOpera.Controller.Panes.DesignPanes
{
    public class DesignerSegmentRowController : IController
    {
        public EventHandler<
            ValueChangedEventArgs<DesignerSegmentRow, SegmentConfiguration>>? ConfigurationChanged { get; set; }
        public EventHandler<ValueEventArgs<IElementController>>? CellSelected { get; set; }

        private DesignerSegmentRow? _row;

        public void Bind(object @object)
        {
            _row = @object as DesignerSegmentRow;
            _row!.CellAdded += HandleCellAdded;
            _row!.CellRemoved += HandleCellRemoved;
            ((IFormElementController<string, SegmentConfiguration>)
                _row!.ConfigurationSelect.Controller).ValueChanged += HandleConfigurationChanged;
            foreach (var cell in _row!.ComponentCells)
            {
                BindCell(cell);
            }
        }

        public void Unbind()
        {
            foreach (var cell in _row!.ComponentCells)
            {
                UnbindCell(cell);
            }
            ((IFormElementController<string, SegmentConfiguration>)
                _row!.ConfigurationSelect.Controller).ValueChanged += HandleConfigurationChanged;
            _row!.CellRemoved -= HandleCellRemoved;
            _row!.CellAdded -= HandleCellAdded;
            _row = null;
        }

        private void BindCell(DesignerComponentCell cell)
        {
            cell.Controller.Clicked += HandleCellClick;
        }

        private void UnbindCell(DesignerComponentCell cell)
        {
            cell.Controller.Clicked -= HandleCellClick;
        }

        private void HandleCellAdded(object? sender, ElementEventArgs e)
        {
            BindCell((DesignerComponentCell)e.Element);
        }

        private void HandleCellClick(object? sender, MouseButtonClickEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                CellSelected?.Invoke(this, new((IElementController)sender!));
            }
        }

        private void HandleCellRemoved(object? sender, ElementEventArgs e)
        {
            UnbindCell((DesignerComponentCell)e.Element);
        }

        private void HandleConfigurationChanged(object? sender, ValueChangedEventArgs<string, SegmentConfiguration?> e)
        {
            ConfigurationChanged?.Invoke(this, new(_row!, e.Value!));
        }
    }
}
