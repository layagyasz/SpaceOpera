﻿using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Panes.DesignPanes;

namespace SpaceOpera.Controller.Panes.DesignPanes
{
    public class DesignerSegmentTableController : IController
    {
        public EventHandler<
            ValueChangedEventArgs<DesignerSegmentRow, SegmentConfiguration>>? ConfigurationChanged
        { get; set; }
        public EventHandler<ValueEventArgs<IElementController>>? CellSelected { get; set; }

        private UiCompoundComponent? _table;

        public void Bind(object @object)
        {
            _table = @object as UiCompoundComponent;
            _table!.ElementAdded += HandleElementAdded;
            _table!.ElementRemoved += HandleElementRemoved;
            foreach (var row in _table!.Cast<DesignerSegmentRow>())
            {
                BindElement(row);
            }
        }

        public void Unbind()
        {
            foreach (var row in _table!.Cast<DesignerSegmentRow>())
            {
                UnbindElement(row);
            }
            _table!.ElementAdded -= HandleElementAdded;
            _table!.ElementRemoved -= HandleElementRemoved;
            _table = null;
        }

        private void BindElement(DesignerSegmentRow row)
        {
            var controller = (DesignerSegmentRowController)row.ComponentController;
            controller.ConfigurationChanged += HandleConfigurationChange;
            controller.CellSelected += HandleCellSelect;
        }

        private void UnbindElement(DesignerSegmentRow row)
        {
            var controller = (DesignerSegmentRowController)row.ComponentController;
            controller.ConfigurationChanged -= HandleConfigurationChange;
            controller.CellSelected -= HandleCellSelect;
        }

        private void HandleConfigurationChange(
            object? sender, ValueChangedEventArgs<DesignerSegmentRow, SegmentConfiguration> e)
        {
            ConfigurationChanged?.Invoke(this, e);
        }

        private void HandleCellSelect(object? sender, ValueEventArgs<IElementController> e)
        {
            CellSelected?.Invoke(this, e);
        }

        private void HandleElementAdded(object? sender, ElementEventArgs e)
        {
            BindElement((DesignerSegmentRow)e.Element);
        }

        private void HandleElementRemoved(object? sender, ElementEventArgs e)
        {
            UnbindElement((DesignerSegmentRow)e.Element);
        }
    }
}
