using Cardamom.Collections;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Game.Panes.DiplomacyPanes;
using SpaceOpera.Core.Politics.Diplomacy;
using SpaceOpera.View.Components;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class DiplomaticAgreementSideComponent : UiCompoundComponent
    {
        private static readonly string s_Container = "diplomacy-pane-diplomacy-side-container";
        private static readonly string s_SimpleSection = "diplomacy-pane-diplomacy-side-simple-section";

        private static readonly string s_PeaceAgreement = "Peace Treaty";
        private static readonly string s_WarDeclaration = "Declare War";

        private readonly EnumSet<DiplomacyType> _range = new();
        private readonly UiElementFactory _uiElementFactory;

        public DiplomaticAgreementSideComponent(UiElementFactory uiElementFactory)
            : base(new AdderComponentController<IDiplomaticAgreementSection>())
        {
            _uiElementFactory = uiElementFactory;

            SetContainer(
                new DynamicKeyedTable<DiplomacyType, IKeyedUiElement<DiplomacyType>>(
                      uiElementFactory.GetClass(s_Container),
                      new TableController(10f),
                      UiSerialContainer.Orientation.Vertical,
                      GetRange,
                      CreateRow,
                      Comparer<DiplomacyType>.Create((x, y) => (int)x - (int)y)));
        }

        public void SetRange(IEnumerable<DiplomacyType> range)
        {
            _range.Clear();
            foreach (var type in range)
            {
                _range.Add(type);
            }
            ((IDynamic)_container!).Refresh();
        }

        private KeyedUiComponent<DiplomacyType> CreateRow(DiplomacyType diplomacyType)
        {
            return diplomacyType switch
            {
                DiplomacyType.Peace => 
                    KeyedUiComponent<DiplomacyType>.Wrap(
                        diplomacyType,
                        new UiSimpleComponent(
                            new SimpleSectionComponentController(() => new PeaceProposal()),
                            _uiElementFactory.CreateTextButton(s_SimpleSection, s_PeaceAgreement).Item1)),
                DiplomacyType.War =>
                    KeyedUiComponent<DiplomacyType>.Wrap(
                        diplomacyType,
                        new UiSimpleComponent(
                            new SimpleSectionComponentController(() => new PeaceProposal()),
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
