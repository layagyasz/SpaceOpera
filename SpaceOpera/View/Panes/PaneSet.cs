using Cardamom.Ui;
using SpaceOpera.View.Icons;
using SpaceOpera.View.Panes.DesignPanes;
using SpaceOpera.View.Panes.MilitaryPanes;
using SpaceOpera.View.Panes.ResearchPanes;

namespace SpaceOpera.View.Panes
{
    public class PaneSet
    {
        public DesignPane Design { get; }
        public DesignerPane Designer { get; }
        public MultiTabGamePane Military { get; }
        public MultiTabGamePane Research { get; }

        private PaneSet(DesignPane design, DesignerPane designer, MultiTabGamePane military, MultiTabGamePane research)
        {
            Design = design;
            Designer = designer;
            Military = military;
            Research = research;
        }

        public IGamePane Get(GamePaneId id)
        {
            return id switch
            {
                GamePaneId.Design => Design,
                GamePaneId.Designer => Designer,
                GamePaneId.Military => Military,
                GamePaneId.Research => Research,
                _ => throw new ArgumentException($"Unsupported pane id: {id}"),
            }; ;
        }

        public IEnumerable<IGamePane> GetPanes()
        {
            yield return Design;
            yield return Designer;
            yield return Military;
            yield return Research;
        }

        public static PaneSet Create(UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            var design = new DesignPane(uiElementFactory, iconFactory);
            design.Initialize();
            var designer = new DesignerPane(uiElementFactory, iconFactory);
            designer.Initialize();
            var military = MilitaryPane.Create(uiElementFactory);
            military.Initialize();
            var research = ResearchPane.Create(uiElementFactory);
            research.Initialize();
            return new(design, designer, military, research);
        }
    }
}
