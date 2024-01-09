using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.StellarBodyPanes
{
    public class OverviewTab : DynamicUiCompoundComponent
    {
        private static readonly string s_Container = "stellar-body-pane-body";
        private static readonly string s_Column = "stellar-body-pane-overview-column";
        private static readonly string s_Header2Container = "stellar-body-pane-overview-container-h2";
        private static readonly string s_IconTitle = "stellar-body-pane-overview-icon-title";
        private static readonly string s_IconHeader2 = "stellar-body-pane-overview-icon-h2";
        private static readonly string s_Header1 = "stellar-body-pane-overview-h1";
        private static readonly string s_Header2 = "stellar-body-pane-overview-h2";
        private static readonly string s_Header3 = "stellar-body-pane-overview-h3";
        private static readonly string s_Paragraph = "stellar-body-pane-overview-p";

        private static readonly string s_Table = "stellar-body-pane-overview-table";
        private static readonly string s_HoldingContainer = "stellar-body-pane-overview-holding-container";
        private static readonly ChipSetStyles.ChipSetStyle s_ChipSet = new()
        {
            Container = "stellar-body-pane-overview-chip-set",
            Chip = new()
            {
                Container = "stellar-body-pane-overview-chip",
                Icon = "stellar-body-pane-overview-chip-icon",
                Text = "stellar-body-pane-overview-chip-text"
            }
        };

        private StellarBody? _stellarBody;
        private EconomicZoneRoot? _root;
        private EconomicZoneHolding? _holding;

        public OverviewTab(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new NoOpTabController(),
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new NoOpElementController(),
                      UiSerialContainer.Orientation.Horizontal))
        {
            Add(
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Column),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical)
                {
                    new DynamicIcon(
                        uiElementFactory.GetClass(s_IconTitle),
                        new NoOpElementController(),
                        () => _stellarBody,
                        iconFactory,
                        IconResolution.High),
                    new TextUiElement(uiElementFactory.GetClass(s_Header1), new InlayController(), "Surface"),
                    new TextUiElement(uiElementFactory.GetClass(s_Header3), new InlayController(), "Classification"),
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_Paragraph),
                        new InlayController(),
                        () => _stellarBody?.Type ?? string.Empty),
                    new TextUiElement(uiElementFactory.GetClass(s_Header3), new InlayController(), "Mass"),
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_Paragraph),
                        new InlayController(),
                        () => $"{_stellarBody?.MassKg / Constants.EarthMass:N2} e"),
                    new TextUiElement(uiElementFactory.GetClass(s_Header3), new InlayController(), "Radius"),
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_Paragraph),
                        new InlayController(),
                        () => $"{_stellarBody?.RadiusKm:N0} km"),
                    new TextUiElement(uiElementFactory.GetClass(s_Header3), new InlayController(), "Land Cover"),
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_Paragraph),
                        new InlayController(),
                        () => $"{1f * _stellarBody?.Regions.Count(x => x.DominantBiome.IsTraversable)
                        / _stellarBody?.Regions.Count:P0}")
                });
            Add(
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Column),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical)
                {
                    new TextUiElement(uiElementFactory.GetClass(s_Header1), new InlayController(), "Orbit"),
                    new TextUiElement(uiElementFactory.GetClass(s_Header3), new InlayController(), "Solar Distance"),
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_Paragraph),
                        new InlayController(),
                        () => $"{_stellarBody?.Orbit.GetAverageDistance():N2} au"),
                    new TextUiElement(uiElementFactory.GetClass(s_Header3), new InlayController(), "Eccentricity"),
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_Paragraph),
                        new InlayController(),
                        () => $"{_stellarBody?.Orbit.Eccentricity:N2}"),
                    new TextUiElement(
                        uiElementFactory.GetClass(s_Header3), new InlayController(), "Geosynchronous Altitude"),
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_Paragraph),
                        new InlayController(),
                        () => $"{_stellarBody?.GetGeosynchronousOrbitAltitudeKm():N0} km"),
                    new TextUiElement(
                        uiElementFactory.GetClass(s_Header3), new InlayController(), "High Orbit Altitude"),
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_Paragraph),
                        new InlayController(),
                        () => $"{_stellarBody?.GetHighOrbitAltitudeKm():N0} km"),
                    new TextUiElement(
                        uiElementFactory.GetClass(s_Header3), new InlayController(), "Year Length"),
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_Paragraph),
                        new InlayController(),
                        () => $"{_stellarBody?.Orbit.GetYearLengthInSeconds() * Constants.DaysPerSecond:N2} days"),
                    new TextUiElement(
                        uiElementFactory.GetClass(s_Header3), new InlayController(), "Day Length"),
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_Paragraph),
                        new InlayController(),
                        () => $"{_stellarBody?.Orbit.GetDayLengthInSeconds() * Constants.HoursPerSecond:N2} hours")
                });
            Add(
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Column),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical)
                {
                    new TextUiElement(uiElementFactory.GetClass(s_Header1), new InlayController(), "Resources"),
                    DynamicKeyedContainer<IMaterial>.CreateChip(
                        uiElementFactory.GetClass(s_ChipSet.Container!), 
                        new NoOpElementController(), 
                        () => _holding?.GetHoldings().SelectMany(y => y.GetResources()).Distinct()
                            ?? Enumerable.Empty<IMaterial>(), 
                        new UiChip<IMaterial>.Factory(
                            x => _holding?.GetHoldings().Sum(y => y.GetResourceNodes(x)).ToString("N0") 
                                ?? string.Empty,
                            s_ChipSet.Chip!, 
                            uiElementFactory,
                            iconFactory),
                        Comparer<IMaterial>.Create((x, y) => x.Name.CompareTo(y.Name)))
                });
            Add(
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Column),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical)
                {
                    new TextUiElement(uiElementFactory.GetClass(s_Header1), new InlayController(), "Politics"),
                    DynamicKeyedContainer<EconomicZoneHolding>.CreateSerial(
                        uiElementFactory.GetClass(s_Table),
                        new NoOpElementController(), 
                        UiSerialContainer.Orientation.Vertical, 
                        () => _root?.GetHoldings() ?? Enumerable.Empty<EconomicZoneHolding>(),
                        new SimpleKeyedElementFactory<EconomicZoneHolding>(
                            uiElementFactory, iconFactory, CreateHoldingDescription),
                        Comparer<EconomicZoneHolding>.Create((x, y) => x.Owner.Name.CompareTo(y.Owner.Name)))
                });
        }

        public void SetHolding(EconomicZoneRoot root, EconomicZoneHolding holding)
        {
            _holding = holding;
            _root = root;
            _stellarBody = holding.StellarBody;
        }

        private IKeyedUiElement<EconomicZoneHolding> CreateHoldingDescription(
            EconomicZoneHolding holding, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return KeyedUiElement<EconomicZoneHolding>.Wrap(
                holding, 
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_HoldingContainer),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Vertical)
                {
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_Header2Container), 
                        new NoOpElementController(),
                        UiSerialContainer.Orientation.Horizontal)
                    {
                        iconFactory.Create(
                            uiElementFactory.GetClass(s_IconHeader2), new InlayController(), holding.Owner),
                        new TextUiElement(
                            uiElementFactory.GetClass(s_Header2), new InlayController(), holding.Owner.Name),
                    },
                    new TextUiElement(uiElementFactory.GetClass(s_Header3), new InlayController(), "Population"),
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_Paragraph), 
                        new InlayController(), 
                        () => (1f * holding.GetPopulation() / holding.StellarBody.Population.Get()).ToString("P0")),
                    new TextUiElement(uiElementFactory.GetClass(s_Header3), new InlayController(), "Land Mass"),
                    new DynamicTextUiElement(
                        uiElementFactory.GetClass(s_Paragraph),
                        new InlayController(),
                        () => (1f * holding.GetRegionCount(/* isTraversable= */ true) 
                            / holding.StellarBody.GetRegionCount(/* isTraversable= */ true)).ToString("P0"))
                });
        }
    }
}
