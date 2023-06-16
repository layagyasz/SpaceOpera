using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.View.Icons;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.View.Components;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Game.Panes.DiplomacyPanes;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class DiplomacyPane : SimpleGamePane
    {
        private static readonly string s_Container = "diplomacy-pane";
        private static readonly string s_Title = "diplomacy-pane-title";
        private static readonly string s_Close = "diplomacy-pane-close";
        private static readonly string s_Body = "diplomacy-pane-body";

        private static readonly string s_RelationTable = "diplomacy-pane-relation-table";
        private static readonly ActionRow<DiplomaticRelation>.Style s_RelationRowStyle =
            new()
            {
                Container = "diplomacy-pane-relation-row",
                ActionContainer = "diplomacy-pane-relation-row-action-container"
            };
        private static readonly string s_Icon = "diplomacy-pane-relation-row-icon";
        private static readonly string s_Text = "diplomacy-pane-relation-row-text";
        private static readonly string s_Status = "diplomacy-pane-relation-row-status";
        private static readonly List<ActionRow<DiplomaticRelation>.ActionConfiguration> s_RelationActions =
            new()
            {
                new ()
                {
                    Button = "diplomacy-pane-relation-row-action-open",
                    Action = ActionId.Select
                }
            };

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private World? _world;
        private Faction? _faction;

        public UiCompoundComponent Relations { get; }

        public DiplomacyPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new DiplomacyPaneController(),
                  uiElementFactory.GetClass(s_Container),
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), "Diplomacy"),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1)
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            var body = new
                DynamicUiContainer(
                    uiElementFactory.GetClass(s_Body), new NoOpElementController<UiContainer>());

            Relations =
                new DynamicUiCompoundComponent(
                    new ActionComponentController(),
                    new DynamicKeyedTable<DiplomaticRelation, ActionRow<DiplomaticRelation>>(
                        uiElementFactory.GetClass(s_RelationTable),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        GetRange,
                        CreateRow,
                        Comparer<DiplomaticRelation>.Create((x, y) => x.Faction.Name.CompareTo(y.Faction.Name))));

            body.Add(Relations);
            SetBody(body);
        }

        public override void Populate(params object?[] args)
        {
            _world = args[0] as World;
            _faction = args[1] as Faction;
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }

        private ActionRow<DiplomaticRelation> CreateRow(DiplomaticRelation relation)
        {
            return ActionRow<DiplomaticRelation>.Create(
                relation,
                ActionId.Unknown,
                _uiElementFactory,
                s_RelationRowStyle,
                new List<IUiElement>()
                {
                    _iconFactory.Create(_uiElementFactory.GetClass(s_Icon), new InlayController(), relation.Faction),
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_Text), new InlayController(), relation.Faction.Name),
                    new DynamicTextUiElement(
                        _uiElementFactory.GetClass(s_Status),
                        new InlayController(), 
                        () => EnumMapper.ToString(relation.Status))
                },
                s_RelationActions);
        }

        private IEnumerable<DiplomaticRelation> GetRange()
        {
            if (_world == null || _faction == null)
            {
                return Enumerable.Empty<DiplomaticRelation>();
            }
            return _world.DiplomaticRelations.GetAsTarget(_faction);
        }
    }
}
