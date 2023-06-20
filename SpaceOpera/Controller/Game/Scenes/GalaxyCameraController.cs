using Cardamom.Graphics.Camera;
using Cardamom.Mathematics;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui;
using Cardamom.Window;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;

namespace SpaceOpera.Controller.Game.Scenes
{
    public class GalaxyCameraController : IElementController
    {
        public EventHandler<MouseButtonClickEventArgs>? Clicked { get; set; }
        public EventHandler<EventArgs>? Focused { get; set; }
        public EventHandler<EventArgs>? FocusLeft { get; set; }
        public EventHandler<EventArgs>? MouseEntered { get; set; }
        public EventHandler<EventArgs>? MouseLeft { get; set; }

        public float KeySensitivity { get; set; } = 1f;
        public float MouseWheelSensitivity { get; set; } = 1f;
        public Interval PitchRange { get; set; } = Interval.Unbounded;
        public Interval DistanceRange { get; set; } = Interval.Unbounded;
        public float Radius { get; set; }

        private readonly SubjectiveCamera3d _camera;

        public GalaxyCameraController(SubjectiveCamera3d camera)
        {
            _camera = camera;
        }

        public void Bind(object @object) { }

        public void Unbind() { }

        public bool HandleKeyDown(KeyDownEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.Left:
                    ChangeFocus(new(KeySensitivity * e.TimeDelta, 0, 0));
                    return true;
                case Keys.Right:
                    ChangeFocus(new(-KeySensitivity * e.TimeDelta, 0, 0));
                    return true;
                case Keys.Up:
                    ChangeFocus(new(0, 0, KeySensitivity * e.TimeDelta));
                    return true;
                case Keys.Down:
                    ChangeFocus(new(0, 0, -KeySensitivity * e.TimeDelta));
                    return true;
            }
            return false;
        }

        private void ChangeFocus(Vector3 delta)
        {
            var newFocus = _camera.Focus + delta;
            if (Vector3.Dot(newFocus, newFocus) > Radius * Radius)
            {
                newFocus = Radius * newFocus.Normalized();
            }
            _camera.SetFocus(newFocus);
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
            return false;
        }

        public bool HandleMouseButtonDragged(MouseButtonDragEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                ChangeFocus(2 * _camera.Distance * new Vector3(_camera.AspectRatio * e.NdcDelta.X, 0, -e.NdcDelta.Y));
                return true;
            }
            return false;
        }

        public bool HandleMouseWheelScrolled(MouseWheelEventArgs e)
        {
            _camera.SetDistance(DistanceRange.Clamp(_camera.Distance - MouseWheelSensitivity * e.OffsetY));
            float p = 1 - (_camera.Distance - DistanceRange.Minimum) / (DistanceRange.Maximum - DistanceRange.Minimum);
            p = p * p * p * p;
            _camera.SetPitch(PitchRange.Minimum + (PitchRange.Maximum - PitchRange.Minimum) * p);
            return true;
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
