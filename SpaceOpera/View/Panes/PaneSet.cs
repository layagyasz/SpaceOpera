using Cardamom.Ui;
using SpaceOpera.View.Icons;
using SpaceOpera.View.Panes.DesignPanes;
using SpaceOpera.View.Panes.MilitaryPanes;
using SpaceOpera.View.Panes.ResearchPanes;

namespace SpaceOpera.View.Panes
{
    public class PaneSet
    {
        public GamePane Design { get; }
        public GamePane Military { get; }
        public GamePane Research { get; }

        private PaneSet(GamePane design, GamePane military, GamePane research)
        {
            Design = design;
            Military = military;
            Research = research;
        }

        public GamePane Get(GamePaneId id)
        {
            return id switch
            {
                GamePaneId.Design => Design,
                GamePaneId.Military => Military,
                GamePaneId.Research => Research,
                _ => throw new ArgumentException($"Unsupported pane id: {id}"),
            };
        }

        public IEnumerable<GamePane> GetPanes()
        {
            yield return Design;
            yield return Military;
            yield return Research;
        }

        public static PaneSet Create(UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            var design = new DesignPane(uiElementFactory, iconFactory);
            design.Initialize();
            var military = MilitaryPane.Create(uiElementFactory);
            military.Initialize();
            var research = ResearchPane.Create(uiElementFactory);
            research.Initialize();
            return new(design, military, research);
        }
    }
}
