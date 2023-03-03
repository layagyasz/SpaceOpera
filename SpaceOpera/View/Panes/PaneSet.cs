using Cardamom.Ui;
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
            switch (id)
            {
                case GamePaneId.Design:
                    return Design;
                case GamePaneId.Military:
                    return Military;
                case GamePaneId.Research:
                    return Research;
                default:
                    throw new ArgumentException($"Unsupported pane id: {id}");
            }
        }

        public static PaneSet Create(UiElementFactory uiElementFactory)
        {
            var design = new DesignPane(uiElementFactory);
            design.Initialize();
            var military = MilitaryPane.Create(uiElementFactory);
            military.Initialize();
            var research = ResearchPane.Create(uiElementFactory);
            research.Initialize();
            return new(design, military, research);
        }
    }
}
