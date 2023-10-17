using Cardamom.Collections;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes.DiplomacyPanes;
using SpaceOpera.Core.Politics.Diplomacy;
using SpaceOpera.View.Components;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class DiplomaticAgreementOptionsComponent : UiCompoundComponent
    {
        private static readonly string s_Container = "diplomacy-pane-diplomacy-side-container";
        private static readonly string s_Table = "diplomacy-pane-diplomacy-side-table";
        private static readonly string s_Header = "diplomacy-pane-diplomacy-side-header";
        private static readonly string s_SimpleSection = "diplomacy-pane-diplomacy-side-simple-section";

        private static readonly string s_DefensePact = "Defense Pact";
        private static readonly string s_PeaceAgreement = "Peace Treaty";
        private static readonly string s_WarDeclaration = "Declare War";

        private readonly EnumSet<DiplomacyType> _range = new();
        private readonly UiElementFactory _uiElementFactory;

        public IUiComponent Options { get; }

        public DiplomaticAgreementOptionsComponent(string header, UiElementFactory uiElementFactory)
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
                    new DynamicKeyedTable<DiplomacyType, IKeyedUiElement<DiplomacyType>>(
                          uiElementFactory.GetClass(s_Table),
                          new TableController(10f),
                          UiSerialContainer.Orientation.Vertical,
                          GetRange,
                          CreateRow,
                          Comparer<DiplomacyType>.Create((x, y) => (int)x - (int)y)));
            Add(Options);
        }

        public void SetRange(IEnumerable<DiplomacyType> range)
        {
            _range.Clear();
            foreach (var type in range)
            {
                _range.Add(type);
            }
            ((IDynamic)Options).Refresh();
        }

        private KeyedUiComponent<DiplomacyType> CreateRow(DiplomacyType diplomacyType)
        {
            return diplomacyType switch
            {
                DiplomacyType.DefensePact =>
                    KeyedUiComponent<DiplomacyType>.Wrap(
                        diplomacyType,
                        new UiSimpleComponent(
                            new SimpleOptionComponentController(() => new DefensePact()),
                            _uiElementFactory.CreateTextButton(s_SimpleSection, s_DefensePact).Item1)),
                DiplomacyType.Peace => 
                    KeyedUiComponent<DiplomacyType>.Wrap(
                        diplomacyType,
                        new UiSimpleComponent(
                            new SimpleOptionComponentController(() => new PeaceProposal()),
                            _uiElementFactory.CreateTextButton(s_SimpleSection, s_PeaceAgreement).Item1)),
                DiplomacyType.War =>
                    KeyedUiComponent<DiplomacyType>.Wrap(
                        diplomacyType,
                        new UiSimpleComponent(
                            new SimpleOptionComponentController(() => new WarDeclaration()),
                            _uiElementFactory.CreateTextButton(s_SimpleSection, s_WarDeclaration).Item1)),
                _ => throw new ArgumentException($"Unsupported DiplomacyType: [{diplomacyType}]"),
            };
        }

        private IEnumerable<DiplomacyType> GetRange()
        {
            return _range;
        }
    }
}
