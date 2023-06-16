using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Icons;
using Cardamom.Ui.Controller;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.Controller.Game.Panes.DesignPanes;
using Cardamom.Collections;
using SpaceOpera.View.Game.Info;

namespace SpaceOpera.View.Game.Panes.DesignPanes
{
    public class DesignerPane : SimpleGamePane
    {
        private static readonly string s_Container = "designer-pane";
        private static readonly string s_Title = "designer-pane-title";
        private static readonly string s_Close = "designer-pane-close";
        private static readonly string s_Body = "designer-pane-body";
        private static readonly string s_ComponentOptionTable = "designer-pane-component-option-table";
        private static readonly string s_SegmentTable = "designer-pane-segment-table";
        private static readonly InfoPanel.Style s_InfoPaneStyle =
            new()
            {
                Container = "designer-pane-info-container",
                Row = "designer-pane-info-row",
                RowHeading = "designer-pane-info-heading",
                RowValue = "designer-pane-info-value",
                MaterialCell = "designer-pane-info-material-cell",
                MaterialIcon = "designer-pane-info-material-icon",
                MaterialText = "designer-pane-info-material-text"
            };

        public UiCompoundComponent ComponentOptionTable { get; }
        public UiCompoundComponent SegmentTable { get; }
        public InfoPanel InfoPanel { get; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private World? _world;
        private Faction? _faction;
        private DesignTemplate? _template;

        public DesignerPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                new DesignerPaneController(),
                uiElementFactory.GetClass(s_Container),
                new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), string.Empty),
                uiElementFactory.CreateSimpleButton(s_Close).Item1)
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            var body = new 
                UiSerialContainer(
                    uiElementFactory.GetClass(s_Body),
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Horizontal);

            ComponentOptionTable =
                new UiCompoundComponent(
                    new RadioController<IComponent>(),
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_ComponentOptionTable),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical));
            body.Add(ComponentOptionTable);

            SegmentTable =
                new UiCompoundComponent(
                    new DesignerSegmentTableController(),
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_SegmentTable),
                        new TableController(0f),
                        UiSerialContainer.Orientation.Vertical));
            body.Add(SegmentTable);

            InfoPanel = new(s_InfoPaneStyle, uiElementFactory, iconFactory);
            body.Add(InfoPanel);

            SetBody(body);
        }

        public DesignBuilder GetDesignBuilder()
        {
            return _world!.DesignBuilder;
        }

        public DesignTemplate GetTemplate()
        {
            return _template!;
        }

        public override void Populate(params object?[] args)
        {
            var design = args[2] as Design;

            _world = args[0] as World;
            _faction = args[1] as Faction;
            _template = design?.Configuration.Template ?? (DesignTemplate)args[3]!;
            
            SetTitle(EnumMapper.ToString(_template.Type));

            ComponentOptionTable.Clear(true);
            SegmentTable.Clear(true);

            if (design == null)
            {
                foreach (var segment in _template.Segments)
                {
                    var segmentRow = new DesignerSegmentRow(segment, _uiElementFactory, _iconFactory);
                    segmentRow.Initialize();
                    var configuration = 
                        ((SelectController<SegmentConfiguration>)segmentRow.ConfigurationSelect.Controller).GetValue();
                    SetSegmentConfiguration(segmentRow, configuration!);
                    SegmentTable.Add(segmentRow);
                }
            }
            else
            {
                foreach (var segment in design.Configuration.GetSegments())
                {
                    var segmentRow = new DesignerSegmentRow(segment.Template, _uiElementFactory, _iconFactory);
                    segmentRow.Initialize();
                    segmentRow.Populate(segment.Configuration, segment.GetComponents());
                    ((SelectController<SegmentConfiguration>)segmentRow.ConfigurationSelect.Controller)
                        .SetValue(segment.Configuration);
                    SegmentTable.Add(segmentRow);
                }
            }
            Populated?.Invoke(this, EventArgs.Empty);
        }

        public void SetInfo(object @object)
        {
            InfoPanel.Clear(true);
            new DesignDescriber().Describe(@object, InfoPanel);
        }

        public void SetSegmentConfiguration(DesignerSegmentRow segmentRow, SegmentConfiguration configuration)
        {
            var defaultComponents = new MultiMap<DesignSlot, IComponent>();
            foreach (var slot in configuration.Slots)
            {
                defaultComponents.Add(
                    slot,
                    Enumerable.Repeat(
                        _world!.GetComponentsFor(_faction!).Where(x => x.FitsSlot(slot)).First(), slot.Count));
            }
            segmentRow.Populate(configuration, defaultComponents);
        }

        public void SetSlot(DesignSlot? slot)
        {
            ComponentOptionTable.Clear(true);
            if (slot != null)
            {
                foreach (var component in _world!.GetComponentsFor(_faction!).Where(x => x.FitsSlot(slot.Value)))
                {
                    var option = DesignerComponentOption.Create(component, _uiElementFactory, _iconFactory);
                    option.Initialize();
                    ComponentOptionTable.Add(option);
                }
            }
        }
    }
}
