using Cardamom.Collections;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Panes.DesignPanes;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.DesignPanes
{
    public class DesignerSegmentRow : UiCompoundComponent
    {
        private static readonly string s_ClassName = "designer-pane-segment-container";
        private static readonly string s_ConfigurationSelectWrapperClassName =
            "designer-pane-segment-configuration-select-wrapper";
        private static readonly string s_ConfigurationSelectClassName = "designer-pane-segment-configuration-select";
        private static readonly string s_ConfigurationSelectDropBoxClassName = 
            "designer-pane-segment-configuration-select-dropbox";
        private static readonly string s_ConfigurationSelectDropOptionClassName =
            "designer-pane-segment-configuration-select-option";
        private static readonly string s_ComponentTableClassName = "designer-pane-segment-component-table";
        private static readonly string s_ComponentRowClassName = "designer-pane-segment-component-row";
        private static readonly string s_ComponentCellClassName = "designer-pane-segment-component-cell";
        private static readonly string s_ComponentIconClassName = "designer-pane-segment-component-icon";

        private static readonly int s_ComponentRowElementCount = 8;

        public EventHandler<ElementEventArgs>? CellAdded { get; set; }
        public EventHandler<ElementEventArgs>? CellRemoved { get; set; }

        public IUiElement ConfigurationSelect { get; }
        public IUiContainer ComponentTable { get; }
        public List<DesignerComponentCell> ComponentCells { get; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        public DesignerSegmentRow(SegmentTemplate template, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new DesignerSegmentRowController(),
                  new UiSerialContainer(
                      uiElementFactory.GetClass(s_ClassName), 
                      new ButtonController(),
                      UiSerialContainer.Orientation.Vertical))
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            ConfigurationSelect = 
                uiElementFactory.CreateSelect<SegmentConfiguration>(
                    s_ConfigurationSelectClassName,
                    s_ConfigurationSelectDropBoxClassName,
                    template.ConfigurationOptions.Select(
                        x => uiElementFactory.CreateSelectOption(
                            s_ConfigurationSelectDropOptionClassName, x, x.Name).Item1)).Item1;
            Add(
                new UiWrapper(
                    uiElementFactory.GetClass(s_ConfigurationSelectWrapperClassName), 
                    new ButtonController(), 
                    ConfigurationSelect));

            ComponentTable = 
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_ComponentTableClassName), 
                    new ButtonController(),
                    UiSerialContainer.Orientation.Vertical);
            Add(ComponentTable);

            ComponentCells = new();
        }

        public void Populate(SegmentConfiguration configuration, MultiMap<DesignSlot, IComponent> components)
        {
            foreach (var cell in ComponentCells)
            {
                CellRemoved?.Invoke(this, new(cell));
            }
            ComponentCells.Clear();
            ComponentTable.Clear();

            var cells = new List<DesignerComponentCell>();
            foreach (var configSlot in configuration.Slots)
            {
                var c = components[configSlot].ToList();
                for (int i = 0; i < configSlot.Count; ++i)
                {
                    var controller = new DesignerComponentCellController(configSlot);
                    var slot = 
                        new DesignerComponentCell(
                            _uiElementFactory.GetClass(s_ComponentCellClassName),
                            controller,
                            _iconFactory, 
                            _uiElementFactory.GetClass(s_ComponentIconClassName));
                    slot.Initialize();
                    controller.SetValue(c[i]);
                    cells.Add(slot);
                }
            }

            foreach (var chunk in cells.Chunk(s_ComponentRowElementCount))
            {
                var row = 
                    new UiSerialContainer(
                        _uiElementFactory.GetClass(s_ComponentRowClassName), 
                        new ButtonController(), 
                        UiSerialContainer.Orientation.Horizontal);
                row.Initialize();
                foreach (var cell in chunk)
                {
                    row.Add(cell);
                    CellAdded?.Invoke(this, new(cell));
                }
                ComponentTable.Add(row);
            }
            ComponentCells.AddRange(cells);
        }
    }
}
