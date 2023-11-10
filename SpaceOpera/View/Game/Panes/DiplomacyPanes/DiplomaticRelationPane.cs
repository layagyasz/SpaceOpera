using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.View.Icons;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.View.Components;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Game.Panes.DiplomacyPanes;
using SpaceOpera.View.Components.Dynamics;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class DiplomaticRelationPane : SimpleGamePane
    {
        private static readonly string s_Container = "diplomatic-relation-pane";
        private static readonly string s_Title = "diplomatic-relation-pane-title";
        private static readonly string s_Close = "diplomatic-relation-pane-close";
        private static readonly string s_Body = "diplomatic-relation-pane-body";

        private static readonly string s_RelationTable = "diplomatic-relation-pane-relation-table";
        private static readonly ActionRow<DiplomaticRelation>.Style s_RelationRowStyle =
            new()
            {
                Container = "diplomatic-relation-pane-relation-row",
                ActionContainer = "diplomatic-relation-pane-relation-row-action-container"
            };
        private static readonly string s_Icon = "diplomatic-relation-pane-relation-row-icon";
        private static readonly string s_Text = "diplomatic-relation-pane-relation-row-text";
        private static readonly string s_Status = "diplomatic-relation-pane-relation-row-status";
        private static readonly string s_Approval = "diplomatic-relation-pane-relation-row-approval";
        private static readonly List<ActionRow<DiplomaticRelation>.ActionConfiguration> s_RelationActions =
            new()
            {
                new ()
                {
                    Button = "diplomatic-relation-pane-relation-row-action-open",
                    Action = ActionId.Select
                }
            };

        class DiplomaticRelationRange : IRange<DiplomaticRelation>
        {
            public World? World { get; set; }
            public Faction? Faction { get; set; }

            public IEnumerable<DiplomaticRelation> GetRange()
            {
                if (World == null || Faction == null)
                {
                    return Enumerable.Empty<DiplomaticRelation>();
                }
                return World.DiplomaticRelations.Get(Faction);
            }
        }

        class DiplomaticRelationComponentFactory : IKeyedElementFactory<DiplomaticRelation>
        {
            private readonly UiElementFactory _uiElementFactory;
            private readonly IconFactory _iconFactory;

            public World? World { get; set; }

            public DiplomaticRelationComponentFactory(UiElementFactory uiElementFactory, IconFactory iconFactory)
            {
                _uiElementFactory = uiElementFactory;
                _iconFactory = iconFactory;
            }

            public IKeyedUiElement<DiplomaticRelation> Create(DiplomaticRelation relation)
            {
                return ActionRow<DiplomaticRelation>.Create(
                    relation,
                    ActionId.Unknown,
                    ActionId.Unknown,
                    _uiElementFactory,
                    s_RelationRowStyle,
                    new List<IUiElement>()
                    {
                    _iconFactory.Create(_uiElementFactory.GetClass(s_Icon), new InlayController(), relation.Target),
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_Text), new InlayController(), relation.Target.Name),
                    new DynamicTextUiElement(
                        _uiElementFactory.GetClass(s_Status),
                        new InlayController(),
                        () => EnumMapper.ToString(relation.OverallStatus)),
                    new DynamicTextUiElement(
                        _uiElementFactory.GetClass(s_Approval),
                        new InlayController(),
                        () => World?.Players
                            .Get(relation.Target)
                            .GetApproval(relation.Faction)?.Result
                            .ToString("N0") ?? "0")
                    },
                    s_RelationActions);
            }
        }

        public UiCompoundComponent Relations { get; }

        private readonly DiplomaticRelationRange _range = new();
        private readonly DiplomaticRelationComponentFactory _elementFactory;

        public DiplomaticRelationPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new DiplomaticRelationPaneController(),
                  uiElementFactory.GetClass(s_Container),
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), "Diplomacy"),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1)
        {
            _elementFactory = new DiplomaticRelationComponentFactory(uiElementFactory, iconFactory);

            var body = new
                DynamicUiContainer(
                    uiElementFactory.GetClass(s_Body), new NoOpElementController());

            Relations =
                new DynamicUiCompoundComponent(
                    new ActionComponentController(),
                    new DynamicKeyedTable<DiplomaticRelation>(
                        uiElementFactory.GetClass(s_RelationTable),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        _range,
                        _elementFactory,
                        Comparer<DiplomaticRelation>.Create((x, y) => x.Faction.Name.CompareTo(y.Faction.Name))));

            body.Add(Relations);
            SetBody(body);
        }

        public override void Populate(params object?[] args)
        {
            _range.World = args[0] as World;
            _range.Faction = args[1] as Faction;
            _elementFactory.World = _range.World;
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }
    }
}
