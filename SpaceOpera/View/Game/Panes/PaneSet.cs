using Cardamom.Ui;
using SpaceOpera.View.Game.Panes.BattlePanes;
using SpaceOpera.View.Game.Panes.DesignPanes;
using SpaceOpera.View.Game.Panes.DiplomacyPanes;
using SpaceOpera.View.Game.Panes.FormationPanes;
using SpaceOpera.View.Game.Panes.LogisticsPanes;
using SpaceOpera.View.Game.Panes.MilitaryPanes;
using SpaceOpera.View.Game.Panes.OrderConfirmationPanes;
using SpaceOpera.View.Game.Panes.ResearchPanes;
using SpaceOpera.View.Game.Panes.StellarBodyRegionPanes;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes
{
    public class PaneSet
    {
        public BattlePane Battle { get; }
        public DesignerPane Designer { get; }
        public DiplomacyPane Diplomacy { get; }
        public EquipmentPane Equipment { get; }
        public FormationPane Formation { get; }
        public LogisticsPane Logistics { get; }
        public LogisticsRoutePane LogisticsRoute { get; }
        public MilitaryPane Military { get; }
        public MilitaryOrganizationPane MilitaryOrganization { get; }
        public OrderConfirmationPane OrderConfirmation { get; }
        public ResearchPane Research { get; }
        public StellarBodyRegionPane StellarBodyRegion { get; }

        private PaneSet(
            BattlePane battle,
            DesignerPane designer,
            DiplomacyPane diplomacy,
            EquipmentPane equipment,
            FormationPane formation,
            LogisticsPane logistics,
            LogisticsRoutePane logisticsRoute,
            MilitaryPane military, 
            MilitaryOrganizationPane militaryOrganization,
            OrderConfirmationPane orderConfirmation,
            ResearchPane research,
            StellarBodyRegionPane stellarBodyRegion)
        {
            Battle = battle;
            Designer = designer;
            Diplomacy = diplomacy;
            Equipment = equipment;
            Formation = formation;
            Logistics = logistics;
            LogisticsRoute = logisticsRoute;
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
                GamePaneId.Battle => Battle,
                GamePaneId.Designer => Designer,
                GamePaneId.Diplomacy => Diplomacy,
                GamePaneId.Equipment => Equipment,
                GamePaneId.Formation => Formation,
                GamePaneId.Logistics => Logistics,
                GamePaneId.LogisticsRoute => LogisticsRoute,
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
            yield return Battle;
            yield return Designer;
            yield return Diplomacy;
            yield return Equipment;
            yield return Formation;
            yield return Logistics;
            yield return LogisticsRoute;
            yield return Military;
            yield return MilitaryOrganization;
            yield return OrderConfirmation;
            yield return Research;
            yield return StellarBodyRegion;
        }

        public static PaneSet Create(UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            var battle = new BattlePane(uiElementFactory, iconFactory);
            battle.Initialize();

            var designer = new DesignerPane(uiElementFactory, iconFactory);
            designer.Initialize();

            var diplomacy = new DiplomacyPane(uiElementFactory, iconFactory);
            diplomacy.Initialize();

            var equipment = new EquipmentPane(uiElementFactory, iconFactory);
            equipment.Initialize();

            var formation = new FormationPane(uiElementFactory, iconFactory);
            formation.Initialize();

            var logistics = new LogisticsPane(uiElementFactory, iconFactory);
            logistics.Initialize();

            var logisticsRoute = new LogisticsRoutePane(uiElementFactory, iconFactory);
            logisticsRoute.Initialize();

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
                battle,
                designer,
                diplomacy,
                equipment,
                formation,
                logistics,
                logisticsRoute,
                military,
                militaryOrganization,
                orderConfirmation,
                research,
                stellarBodyRegion);
        }
    }
}
