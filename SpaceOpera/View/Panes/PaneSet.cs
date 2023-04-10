using Cardamom.Ui;
using SpaceOpera.View.Icons;
using SpaceOpera.View.Panes.DesignPanes;
using SpaceOpera.View.Panes.MilitaryPanes;
using SpaceOpera.View.Panes.OrderConfirmationPanes;
using SpaceOpera.View.Panes.ResearchPanes;
using SpaceOpera.View.Panes.StellarBodyRegionPanes;

namespace SpaceOpera.View.Panes
{
    public class PaneSet
    {
        public DesignerPane Designer { get; }
        public EquipmentPane Equipment { get; }
        public MilitaryPane Military { get; }
        public MilitaryOrganizationPane MilitaryOrganization { get; }
        public OrderConfirmationPane OrderConfirmation { get; }
        public ResearchPane Research { get; }
        public StellarBodyRegionPane StellarBodyRegion { get; }

        private PaneSet(
            DesignerPane designer,
            EquipmentPane equipment,
            MilitaryPane military, 
            MilitaryOrganizationPane militaryOrganization,
            OrderConfirmationPane orderConfirmation,
            ResearchPane research,
            StellarBodyRegionPane stellarBodyRegion)
        {
            Designer = designer;
            Equipment = equipment;
            Military = military;
            MilitaryOrganization = militaryOrganization;
            OrderConfirmation = orderConfirmation;
            Research = research;
            StellarBodyRegion = stellarBodyRegion;
        }

        public IGamePane Get(GamePaneId id)
        {
            return id switch
            {
                GamePaneId.Designer => Designer,
                GamePaneId.Equipment => Equipment,
                GamePaneId.Military => Military,
                GamePaneId.MilitaryOrganization => MilitaryOrganization,
                GamePaneId.OrderConfirmation => OrderConfirmation,
                GamePaneId.Research => Research,
                GamePaneId.StellarBodyRegion => StellarBodyRegion,
                _ => throw new ArgumentException($"Unsupported pane id: {id}"),
            };
        }

        public IEnumerable<IGamePane> GetPanes()
        {
            yield return Designer;
            yield return Equipment;
            yield return Military;
            yield return MilitaryOrganization;
            yield return OrderConfirmation;
            yield return Research;
            yield return StellarBodyRegion;
        }

        public static PaneSet Create(UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            var designer = new DesignerPane(uiElementFactory, iconFactory);
            designer.Initialize();

            var equipment = new EquipmentPane(uiElementFactory, iconFactory);
            equipment.Initialize();

            var military = new MilitaryPane(uiElementFactory, iconFactory);
            military.Initialize();

            var militaryOrganization = new MilitaryOrganizationPane(uiElementFactory, iconFactory);
            militaryOrganization.Initialize();

            var orderConfirmation = new OrderConfirmationPane(uiElementFactory, iconFactory);
            orderConfirmation.Initialize();

            var research = ResearchPane.Create(uiElementFactory);
            research.Initialize();

            var stellarBodyRegion = new StellarBodyRegionPane(uiElementFactory, iconFactory);
            stellarBodyRegion.Initialize();

            return new(
                designer, equipment, military, militaryOrganization, orderConfirmation, research, stellarBodyRegion);
        }
    }
}
