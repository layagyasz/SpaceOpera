using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes.DesignPanes;
using SpaceOpera.Core;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;
using SpaceOpera.View.Info;

namespace SpaceOpera.View.Game.Panes.DesignPanes
{
    public abstract class DesignPane : MultiTabGamePane
    {
        private static readonly string s_Container = "design-pane";
        private static readonly string s_Title = "design-pane-title";
        private static readonly string s_Close = "design-pane-close";
        private static readonly string s_TabContainer = "design-pane-tab-container";
        private static readonly string s_TabOption = "design-pane-tab-option";
        private static readonly string s_Body = "design-pane-body";

        private static readonly string s_DesignContainer = "design-pane-design-container";
        private static readonly ActionRowStyles.Style s_DesignHeaderStyle =
            new()
            {
                Container = "design-pane-design-header",
                ActionContainer = "design-pane-design-header-action-container"
            };
        private static readonly string s_DesignHeaderSpace = "design-pane-design-header-space";
        private static readonly List<ActionRowStyles.ActionConfiguration> s_DesignHeaderActions =
            new()
            {
                new()
                {
                    Button = "design-pane-design-header-action-add",
                    Action = ActionId.Add
                }
            };

        private static readonly string s_DesignTable = "design-pane-design-table";
        private static readonly ActionRowStyles.Style s_DesignRowStyle =
            new()
            {
                Container = "design-pane-design-row",
                ActionContainer = "design-pane-design-row-action-container"
            };
        private static readonly string s_Icon = "design-pane-design-row-icon";
        private static readonly string s_Text = "design-pane-design-row-text";
        private static readonly List<ActionRowStyles.ActionConfiguration> s_DesignActions =
            new()
            {
                new ()
                {
                    Button = "design-pane-design-row-action-edit",
                    Action = ActionId.Edit
                }
            };

        private static readonly InfoPanel.Style s_InfoPaneStyle =
            new()
            {
                Container = "design-pane-info-container",
                Row = "design-pane-info-row",
                RowHeading = "design-pane-info-heading",
                RowValue = "design-pane-info-value",
                MaterialCell = "design-pane-info-material-cell",
                MaterialIcon = "design-pane-info-material-icon",
                MaterialText = "design-pane-info-material-text"
            };

        class DesignRange
        {
            public World? World { get; set; }
            public Faction? Faction { get; set; }
            public ComponentType ComponentType { get; set; }

            public IEnumerable<Design> GetRange()
            {
                if (World == null || Faction == null)
                {
                    return Enumerable.Empty<Design>();
                }
                return World.GetDesignsFor(Faction).Where(x => x.Configuration.Template.Type == ComponentType);
            }
        }

        public UiCompoundComponent Designs { get; }
        public InfoPanel InfoPanel { get; }

        private readonly DesignRange _range = new();

        protected DesignPane(
            UiElementFactory uiElementFactory, IconFactory iconFactory, IEnumerable<ComponentType> componentTypes)
            : base(
                new DesignPaneController(),
                uiElementFactory.GetClass(s_Container),
                new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), "Designs"),
                uiElementFactory.CreateSimpleButton(s_Close).Item1,
                TabBar<ComponentType>.Create(
                    componentTypes.Select(x => new TabBar<ComponentType>.Definition(x, EnumMapper.ToString(x))),
                    uiElementFactory.GetClass(s_TabContainer),
                    uiElementFactory.GetClass(s_TabOption)))
        {
            Designs =
                new ActionTable<Design>(
                    uiElementFactory.GetClass(s_DesignContainer),
                    ActionRow<Type>.Create(
                        typeof(Design),
                        ActionId.Unknown,
                        ActionId.Unknown,
                        uiElementFactory,
                        s_DesignHeaderStyle,
                        new List<IUiElement>()
                        {
                            new SimpleUiElement(
                                uiElementFactory.GetClass(s_DesignHeaderSpace), new InlayController())
                        },
                        s_DesignHeaderActions),
                    DynamicKeyedContainer<Design>.CreateSerial(
                        uiElementFactory.GetClass(s_DesignTable),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        _range.GetRange,
                        new SimpleKeyedElementFactory<Design>(uiElementFactory, iconFactory, CreateRow),
                        Comparer<Design>.Create((x, y) => x.Name.CompareTo(y.Name))),
                    /* isSelectable=*/ true);

            InfoPanel = new(s_InfoPaneStyle, uiElementFactory, iconFactory);

            var body = 
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Body),
                    new NoOpElementController(), 
                    UiSerialContainer.Orientation.Horizontal)
                {
                    Designs,
                    InfoPanel
                };
            SetBody(body);
        }

        public override void Populate(params object?[] args)
        {
            _range.World = args[0] as World;
            _range.Faction = args[1] as Faction;
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }

        public override object GetTab()
        {
            return _range.ComponentType;
        }

        public override void SetTab(object id)
        {
            _range.ComponentType = (ComponentType)id;
        }

        public void SetInfo(object? @object)
        {
            InfoPanel.Clear(true);
            if (@object != null)
            {
                new DesignDescriber().Describe(@object, InfoPanel);
            }
        }

        private static IKeyedUiElement<Design> CreateRow(
            Design design, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return ActionRow<Design>.Create(
                design,
                ActionId.Unknown,
                ActionId.Unknown,
                uiElementFactory,
                s_DesignRowStyle,
                new List<IUiElement>()
                {
                        iconFactory.Create(uiElementFactory.GetClass(s_Icon), new InlayController(), design),
                        new TextUiElement(
                            uiElementFactory.GetClass(s_Text), new InlayController(), design.Name)
                },
                s_DesignActions);
        }
    }
}
