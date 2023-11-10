using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Game.Panes.DiplomacyPanes;
using SpaceOpera.Core;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Diplomacy;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class DiplomaticAgreementOptionsComponent : UiCompoundComponent
    {
        public record class OptionKey(World World, DiplomaticRelation Relation, DiplomacyType DiplomacyType);

        private static readonly string s_Container = "diplomacy-pane-diplomacy-side-container";
        private static readonly string s_Table = "diplomacy-pane-diplomacy-side-table";
        private static readonly string s_Header = "diplomacy-pane-diplomacy-side-header";
        private static readonly string s_SimpleSection = "diplomacy-pane-diplomacy-side-simple-section";

        class OptionKeyElementFactory : IKeyedElementFactory<OptionKey>
        {
            private readonly bool _isLeft;
            private readonly UiElementFactory _uiElementFactory;

            public OptionKeyElementFactory(bool isLeft, UiElementFactory uiElementFactory)
            {
                _isLeft = isLeft;
                _uiElementFactory = uiElementFactory;
            }

            public IKeyedUiElement<OptionKey> Create(OptionKey key)
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
                            _isLeft ? key.Relation.Faction : key.Relation.Target,
                            _isLeft ? key.Relation.Target : key.Relation.Faction));
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
        }

        public bool IsLeft { get; }
        public IUiComponent Options { get; }

        private readonly StaticRange<OptionKey> _range = new();

        public DiplomaticAgreementOptionsComponent(
            bool isLeft, string header, UiElementFactory uiElementFactory)
            : base(
                  new DiplomaticAgreementOptionsComponentController(), 
                  new UiSerialContainer(
                      uiElementFactory.GetClass(s_Container), 
                      new NoOpElementController(),
                      UiSerialContainer.Orientation.Vertical))
        {
            Add(uiElementFactory.CreateTextButton(s_Header, header).Item1);

            Options =
                new DynamicUiCompoundComponent(
                    new DiplomaticAgreementOptionsComponentController(),
                    new DynamicKeyedTable<OptionKey>(
                          uiElementFactory.GetClass(s_Table),
                          new TableController(10f),
                          UiSerialContainer.Orientation.Vertical,
                          _range,
                          new OptionKeyElementFactory(isLeft, uiElementFactory),
                          Comparer<OptionKey>.Create((x, y) => 0)));
            Add(Options);
        }

        public void SetRange(IEnumerable<OptionKey> range)
        {
            _range.Set(range);
            ((IDynamic)Options).Refresh();
        }
    }
}
