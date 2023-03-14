using Cardamom.Ui;
using SpaceOpera.View.Icons;
using SpaceOpera.View.Panes.DesignPanes;
using SpaceOpera.View.Panes.MilitaryPanes;
using SpaceOpera.View.Panes.ResearchPanes;

namespace SpaceOpera.View.Panes
{
    public class PaneSet
    {
        public DesignerPane Designer { get; }
        public EquipmentPane Equipment { get; }
        public MultiTabGamePane Military { get; }
        public MilitaryOrganizationPane MilitaryOrganization { get; }
        public MultiTabGamePane Research { get; }

        private PaneSet(
            DesignerPane designer,
            EquipmentPane equipment,
            MultiTabGamePane military, 
            MilitaryOrganizationPane militaryOrganization,
            MultiTabGamePane research)
        {
            Designer = designer;
            Equipment = equipment;
            Military = military;
            MilitaryOrganization = militaryOrganization;
            Research = research;
        }

        public IGamePane Get(GamePaneId id)
        {
            return id switch
            {
                GamePaneId.Designer => Designer,
                GamePaneId.Equipment => Equipment,
                GamePaneId.Military => Military,
                GamePaneId.MilitaryOrganization => MilitaryOrganization,
                GamePaneId.Research => Research,
                _ => throw new ArgumentException($"Unsupported pane id: {id}"),
            };
        }

        public IEnumerable<IGamePane> GetPanes()
        {
            yield return Designer;
            yield return Equipment;
            yield return Military;
            yield return MilitaryOrganization;
            yield return Research;
        }

        public static PaneSet Create(UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            var designer = new DesignerPane(uiElementFactory, iconFactory);
            designer.Initialize();

            var equipment = new EquipmentPane(uiElementFactory, iconFactory);
            equipment.Initialize();

            var military = MilitaryPane.Create(uiElementFactory);
            military.Initialize();

            var militaryOrganization = new MilitaryOrganizationPane(uiElementFactory, iconFactory);
            militaryOrganization.Initialize();

            var research = ResearchPane.Create(uiElementFactory);
            research.Initialize();
            return new(designer, equipment, military, militaryOrganization, research);
        }
    }
}
