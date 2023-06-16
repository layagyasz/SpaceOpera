using Cardamom.Collections;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes.DesignPanes;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.DesignPanes
{
    public class DesignerSegmentRow : UiCompoundComponent
    {
        private static readonly string s_Container = "designer-pane-segment-container";
        private static readonly string s_ConfigurationSelectWrapper =
            "designer-pane-segment-configuration-select-wrapper";
        private static readonly string s_ConfigurationSelect = "designer-pane-segment-configuration-select";
        private static readonly string s_ConfigurationSelectDropBox = 
            "designer-pane-segment-configuration-select-dropbox";
        private static readonly string s_ConfigurationSelectDropOption =
            "designer-pane-segment-configuration-select-option";
        private static readonly string s_ComponentTable = "designer-pane-segment-component-table";
        private static readonly string s_ComponentRow = "designer-pane-segment-component-row";
        private static readonly string s_ComponentCell = "designer-pane-segment-component-cell";
        private static readonly string s_ComponentIcon = "designer-pane-segment-component-icon";

        private static readonly int s_ComponentRowElementCount = 8;

        public EventHandler<ElementEventArgs>? CellAdded { get; set; }
        public EventHandler<ElementEventArgs>? CellRemoved { get; set; }

        public SegmentTemplate Template { get; }
        public IUiElement ConfigurationSelect { get; }
        public IUiContainer ComponentTable { get; }
        public List<DesignerComponentCell> ComponentCells { get; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        public DesignerSegmentRow(SegmentTemplate template, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new DesignerSegmentRowController(),
                  new UiSerialContainer(
                      uiElementFactory.GetClass(s_Container), 
                      new ButtonController(),
                      UiSerialContainer.Orientation.Vertical))
        {
            Template = template;
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            ConfigurationSelect = 
                uiElementFactory.CreateSelect<SegmentConfiguration>(
                    s_ConfigurationSelect,
                    s_ConfigurationSelectDropBox,
                    template.ConfigurationOptions.Select(
                        x => uiElementFactory.CreateSelectOption(
                            s_ConfigurationSelectDropOption, x, x.Name).Item1),
                    10f).Item1;
            Add(
                new UiWrapper(
                    uiElementFactory.GetClass(s_ConfigurationSelectWrapper), 
                    new ButtonController(), 
                    ConfigurationSelect));

            ComponentTable = 
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_ComponentTable), 
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
            ComponentTable.Clear(true);

            var cells = new List<DesignerComponentCell>();
            foreach (var configSlot in configuration.Slots)
            {
                var c = components[configSlot].ToList();
                for (int i = 0; i < configSlot.Count; ++i)
                {
                    var controller = new DesignerComponentCellController(configSlot);
                    var slot = 
                        new DesignerComponentCell(
                            _uiElementFactory.GetClass(s_ComponentCell),
                            controller,
                            _iconFactory, 
                            _uiElementFactory.GetClass(s_ComponentIcon));
                    slot.Initialize();
                    controller.SetValue(c[i]);
                    cells.Add(slot);
                }
            }

            foreach (var chunk in cells.Chunk(s_ComponentRowElementCount))
            {
                var row = 
                    new UiSerialContainer(
                        _uiElementFactory.GetClass(s_ComponentRow), 
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
