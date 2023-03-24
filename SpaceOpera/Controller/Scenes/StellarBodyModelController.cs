using Cardamom.Collections;
using Cardamom.Mathematics.Coordinates;
using Cardamom.Ui;
using Cardamom.Window;
using OpenTK.Windowing.Common;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Controller.Scenes
{
    public class StellarBodyModelController : ISceneController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<MouseButtonClickEventArgs>? Clicked { get; set; }
        public EventHandler<EventArgs>? Focused { get; set; }

        private readonly KdTree<StellarBodySubRegion> _regions;

        private StellarBodyModelController(KdTree<StellarBodySubRegion> regions)
        {
            _regions = regions;
        }

        public static StellarBodyModelController Create(StellarBody stellarBody, float radius)
        {
            var builder = new KdTree<StellarBodySubRegion>.Builder().SetCardinality(3);
            foreach (var region in stellarBody.Regions.SelectMany(x => x.SubRegions))
            {
                var pos = radius * region.Center.Normalized();
                builder.Add(new HyperVector(pos.X, pos.Y, pos.Z), region);
            }
            return new(builder.Build());
        }

        public void Bind(object @object) { }

        public void Unbind() { }

        public bool HandleKeyDown(KeyDownEventArgs e)
        {
            return false;
        }

        public bool HandleTextEntered(TextEnteredEventArgs e)
        {
            return false;
        }

        public bool HandleMouseEntered()
        {
            return false;
        }

        public bool HandleMouseLeft()
        {
            return false;
        }

        public bool HandleMouseButtonClicked(MouseButtonClickEventArgs e)
        {
            Clicked?.Invoke(this, e);
            var system = _regions.GetClosest(new HyperVector(e.Position.X, e.Position.Y, e.Position.Z));
            Interacted?.Invoke(this, UiInteractionEventArgs.Create(system, e.Button));
            return true;
        }

        public bool HandleMouseButtonDragged(MouseButtonDragEventArgs e)
        {
            return false;
        }

        public bool HandleMouseWheelScrolled(MouseWheelEventArgs e)
        {
            return false;
        }

        public bool HandleMouseLingered()
        {
            return false;
        }

        public bool HandleMouseLingerBroken()
        {
            return false;
        }

        public bool HandleFocusEntered()
        {
            return false;
        }

        public bool HandleFocusLeft()
        {
            return false;
        }
    }
}
