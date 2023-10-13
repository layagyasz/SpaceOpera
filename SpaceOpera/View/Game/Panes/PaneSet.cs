using Cardamom;
using Cardamom.Graphics;
using Cardamom.Ui;
using SpaceOpera.View.Game.Panes.BattlePanes;
using SpaceOpera.View.Game.Panes.DesignPanes;
using SpaceOpera.View.Game.Panes.DiplomacyPanes;
using SpaceOpera.View.Game.Panes.FormationPanes;
using SpaceOpera.View.Game.Panes.Forms;
using SpaceOpera.View.Game.Panes.LogisticsPanes;
using SpaceOpera.View.Game.Panes.MilitaryPanes;
using SpaceOpera.View.Game.Panes.OrderConfirmationPanes;
using SpaceOpera.View.Game.Panes.ResearchPanes;
using SpaceOpera.View.Game.Panes.StellarBodyRegionPanes;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes
{
    public class PaneSet : GraphicsResource, IInitializable
    {
        public BattlePane Battle { get; }
        public DesignerPane Designer { get; }
        public DiplomacyPane Diplomacy { get; }
        public DiplomaticRelationPane DiplomaticRelation { get; }
        public EquipmentPane Equipment { get; }
        public FormPane Form { get; }
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
            DiplomaticRelationPane diplomaticRelation,
            EquipmentPane equipment,
            FormPane form,
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
            DiplomaticRelation = diplomaticRelation;
            Equipment = equipment;
            Form = form;
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
                GamePaneId.DiplomaticRelation => DiplomaticRelation,
                GamePaneId.Equipment => Equipment,
                GamePaneId.Form => Form,
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
            yield return DiplomaticRelation;
            yield return Equipment;
            yield return Form;
            yield return Formation;
            yield return Logistics;
            yield return LogisticsRoute;
            yield return Military;
            yield return MilitaryOrganization;
            yield return OrderConfirmation;
            yield return Research;
            yield return StellarBodyRegion;
        }

        public void Initialize()
        {
            foreach (var pane in GetPanes())
            {
                pane.Initialize();
            }
        }

        protected override void DisposeImpl()
        {
            foreach (var pane in GetPanes())
            {
                pane.Dispose();
            }
        }

        public static PaneSet Create(UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return new(
                /* battle= */ new BattlePane(uiElementFactory, iconFactory),
                /* designer= */  new DesignerPane(uiElementFactory, iconFactory),
                /* diplomacy= */ new DiplomacyPane(uiElementFactory, iconFactory),
                /* diplomaticRelation= */ new DiplomaticRelationPane(uiElementFactory, iconFactory),
                /* equipment= */ new EquipmentPane(uiElementFactory, iconFactory),
                /* form= */ new FormPane(uiElementFactory),
                /* formation= */ new FormationPane(uiElementFactory, iconFactory),
                /* logistics= */ new LogisticsPane(uiElementFactory, iconFactory),
                /* logisticsRoute= */ new LogisticsRoutePane(uiElementFactory, iconFactory),
                /* military= */ new MilitaryPane(uiElementFactory, iconFactory),
                /* militaryOrganization= */ new MilitaryOrganizationPane(uiElementFactory, iconFactory),
                /* orderConfirmation= */ new OrderConfirmationPane(uiElementFactory, iconFactory),
                /* research= */ ResearchPane.Create(uiElementFactory),
                /* stellarBodyRegion= */ new StellarBodyRegionPane(uiElementFactory, iconFactory));
        }
    }
}
