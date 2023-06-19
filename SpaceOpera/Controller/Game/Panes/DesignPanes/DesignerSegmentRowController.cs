using Cardamom.Collections;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Game.Panes.DesignPanes;

namespace SpaceOpera.Controller.Game.Panes.DesignPanes
{
    public class DesignerSegmentRowController : IController
    {
        public EventHandler<KeyValuePair<DesignerSegmentRow, SegmentConfiguration>>? ConfigurationChanged { get; set; }
        public EventHandler<ValueEventArgs<IElementController>>? CellSelected { get; set; }

        private DesignerSegmentRow? _row;

        public void Bind(object @object)
        {
            _row = @object as DesignerSegmentRow;
            _row!.CellAdded += HandleCellAdded;
            _row!.CellRemoved += HandleCellRemoved;
            ((IFormElementController<SegmentConfiguration>)
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
            ((IFormElementController<SegmentConfiguration>)
                _row!.ConfigurationSelect.Controller).ValueChanged += HandleConfigurationChanged;
            _row!.CellRemoved -= HandleCellRemoved;
            _row!.CellAdded -= HandleCellAdded;
            _row = null;
        }

        public Segment GetSegment()
        {
            var components = new MultiMap<DesignSlot, IComponent>();
            foreach (
                var cell in _row!.ComponentCells.Select(x => x.Controller).Cast<DesignerComponentCellController>())
            {
                components.Add(cell.Key, cell.GetValue()!);
            }
            return new(
                _row!.Template,
                ((IFormElementController<SegmentConfiguration>)_row!.ConfigurationSelect.Controller).GetValue()!,
                components);
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

        private void HandleConfigurationChanged(object? sender, EventArgs e)
        {
            ConfigurationChanged?.Invoke(
                this, new(_row!, ((IFormElementController<SegmentConfiguration>)sender!).GetValue()!));
        }
    }
}
