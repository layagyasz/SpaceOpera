using Cardamom.Collections;
using Cardamom.Mathematics.Coordinates;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Window;
using OpenTK.Windowing.Common;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Controller.Game.Scenes
{
    public class GalaxyModelController : IActionController, IElementController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<MouseButtonClickEventArgs>? Clicked { get; set; }
        public EventHandler<EventArgs>? Focused { get; set; }
        public EventHandler<EventArgs>? FocusLeft { get; set; }
        public EventHandler<EventArgs>? MouseEntered { get; set; }
        public EventHandler<EventArgs>? MouseLeft { get; set; }


        private readonly KdTree<StarSystem> _systems;

        private GalaxyModelController(KdTree<StarSystem> systems)
        {
            _systems = systems;
        }

        public static GalaxyModelController Create(Galaxy galaxy, float scale)
        {
            var builder = new KdTree<StarSystem>.Builder().SetCardinality(2);
            foreach (var system in galaxy.Systems)
            {
                builder.Add(new HyperVector(scale * system.Position.X, scale * system.Position.Z), system);
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
            var system = _systems.GetClosest(new HyperVector(e.Position.X, e.Position.Z));
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
