using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Panes.DesignPanes;
using SpaceOpera.Core;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;
using SpaceOpera.View.Info;

namespace SpaceOpera.View.Panes.DesignPanes
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
        private static readonly ActionRow<Type>.Style s_DesignHeaderStyle =
            new()
            {
                Container = "design-pane-design-header",
                ActionContainer = "design-pane-design-header-action-container"
            };
        private static readonly string s_DesignHeaderSpace = "design-pane-design-header-space";
        private static readonly List<ActionRow<Type>.ActionConfiguration> s_DesignHeaderActions =
            new()
            {
                new()
                {
                    Button = "design-pane-design-header-action-add",
                    Action = ActionId.Add
                }
            };

        private static readonly string s_DesignTable = "design-pane-design-table";
        private static readonly ActionRow<Design>.Style s_DesignRowStyle =
            new()
            {
                Container = "design-pane-design-row",
                ActionContainer = "design-pane-design-row-action-container"
            };
        private static readonly string s_Icon = "design-pane-design-row-icon";
        private static readonly string s_Text = "design-pane-design-row-text";
        private static readonly List<ActionRow<Design>.ActionConfiguration> s_DesignActions =
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

        private World? _world;
        private Faction? _faction;
        private ComponentType _componentType;

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        public UiCompoundComponent DesignTable { get; }
        public InfoPanel InfoPanel { get; }

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
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            DesignTable =
                new ActionTable<Design>(
                    _uiElementFactory.GetClass(s_DesignContainer),
                    ActionRow<Type>.Create(
                        typeof(Design),
                        ActionId.Unknown,
                        uiElementFactory,
                        s_DesignHeaderStyle,
                        new List<IUiElement>()
                        {
                            new SimpleUiElement(
                                uiElementFactory.GetClass(s_DesignHeaderSpace), new InlayController())
                        },
                        s_DesignHeaderActions),
                    new DynamicKeyedTable<Design, ActionRow<Design>>(
                        uiElementFactory.GetClass(s_DesignTable),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        GetRange,
                        CreateRow,
                        Comparer<Design>.Create((x, y) => x.Name.CompareTo(y.Name))));

            InfoPanel = new(s_InfoPaneStyle, uiElementFactory, iconFactory);

            var body = 
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Body),
                    new NoOpElementController<UiSerialContainer>(), 
                    UiSerialContainer.Orientation.Horizontal)
                {
                    DesignTable,
                    InfoPanel
                };
            SetBody(body);
        }

        public override void Populate(params object?[] args)
        {
            _world = args[0] as World;
            _faction = args[1] as Faction;
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }

        public override object GetTab()
        {
            return _componentType;
        }

        public override void SetTab(object id)
        {
            _componentType = (ComponentType)id;
        }

        public void SetInfo(object? @object)
        {
            InfoPanel.Clear(true);
            if (@object != null)
            {
                new DesignDescriber().Describe(@object, InfoPanel);
            }
        }

        private ActionRow<Design> CreateRow(Design design)
        {
            return ActionRow<Design>.Create(
                design,
                ActionId.Unknown,
                _uiElementFactory,
                s_DesignRowStyle, 
                new List<IUiElement>() 
                {
                    _iconFactory.Create(_uiElementFactory.GetClass(s_Icon), new InlayController(), design), 
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_Text), new InlayController(), design.Name)
                }, 
                s_DesignActions);
        }

        private IEnumerable<Design> GetRange()
        {
            if (_world == null || _faction == null)
            {
                return Enumerable.Empty<Design>();
            }
            return _world.GetDesignsFor(_faction).Where(x => x.Configuration.Template.Type == _componentType);
        }
    }
}
