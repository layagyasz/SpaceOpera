using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Game.Panes.DiplomacyPanes;
using SpaceOpera.Core;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Diplomacy;
using SpaceOpera.View.Components;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class DiplomaticAgreementOptionsComponent : UiCompoundComponent
    {
        public record class OptionKey(World World, DiplomaticRelation Relation, DiplomacyType DiplomacyType);

        private static readonly string s_Container = "diplomacy-pane-diplomacy-side-container";
        private static readonly string s_Table = "diplomacy-pane-diplomacy-side-table";
        private static readonly string s_Header = "diplomacy-pane-diplomacy-side-header";
        private static readonly string s_SimpleSection = "diplomacy-pane-diplomacy-side-simple-section";

        private readonly HashSet<OptionKey> _range = new();
        private readonly UiElementFactory _uiElementFactory;

        public bool IsLeft { get; }
        public IUiComponent Options { get; }

        public DiplomaticAgreementOptionsComponent(bool isLeft, string header, UiElementFactory uiElementFactory)
            : base(
                  new DiplomaticAgreementOptionsComponentController(), 
                  new UiSerialContainer(
                      uiElementFactory.GetClass(s_Container), 
                      new NoOpElementController(),
                      UiSerialContainer.Orientation.Vertical))
        {
            _uiElementFactory = uiElementFactory;

            Add(uiElementFactory.CreateTextButton(s_Header, header).Item1);

            Options =
                new DynamicUiCompoundComponent(
                    new DiplomaticAgreementOptionsComponentController(),
                    new DynamicKeyedTable<OptionKey, IKeyedUiElement<OptionKey>>(
                          uiElementFactory.GetClass(s_Table),
                          new TableController(10f),
                          UiSerialContainer.Orientation.Vertical,
                          GetRange,
                          CreateRow,
                          Comparer<OptionKey>.Create((x, y) => 0)));
            Add(Options);
        }

        public void SetRange(IEnumerable<OptionKey> range)
        {
            _range.Clear();
            foreach (var type in range)
            {
                _range.Add(type);
            }
            ((IDynamic)Options).Refresh();
        }

        private KeyedUiComponent<OptionKey> CreateRow(OptionKey key)
        {
            var diplomacyType = key.DiplomacyType;
            if (diplomacyType == DiplomacyType.DefensePact)
            {
                return KeyedUiComponent<OptionKey>.Wrap(
                    key,
                    new UiSimpleComponent(
                        new SimpleOptionComponentController(() => new DefensePact()),
                        _uiElementFactory.CreateTextButton(s_SimpleSection, diplomacyType.Name).Item1));
            }
            if (diplomacyType == DiplomacyType.Peace)
            {
                return KeyedUiComponent<OptionKey>.Wrap(
                    key,
                    new UiSimpleComponent(
                        new SimpleOptionComponentController(() => new PeaceProposal()),
                        _uiElementFactory.CreateTextButton(s_SimpleSection, diplomacyType.Name).Item1));
            }
            if (diplomacyType == DiplomacyType.Trade)
            {
                return KeyedUiComponent<OptionKey>.Wrap(
                    key, 
                    TradeComponent.Create(
                        _uiElementFactory, 
                        key.World, 
                        IsLeft ? key.Relation.Faction : key.Relation.Target,
                        IsLeft ? key.Relation.Target : key.Relation.Faction));
            }
            if (diplomacyType == DiplomacyType.War)
            {
                return KeyedUiComponent<OptionKey>.Wrap(
                    key,
                    new UiSimpleComponent(
                        new SimpleOptionComponentController(() => new WarDeclaration()),
                        _uiElementFactory.CreateTextButton(s_SimpleSection, diplomacyType.Name).Item1));
            }
            throw new ArgumentException($"Unsupported DiplomacyType: [{diplomacyType}]");
        }

        private IEnumerable<OptionKey> GetRange()
        {
            return _range;
        }
    }
}
