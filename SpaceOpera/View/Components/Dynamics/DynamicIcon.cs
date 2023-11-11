using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Components.Dynamics
{
    public class DynamicIcon : GraphicsResource, IDynamic, IUiElement
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }
        public float? OverrideDepth { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Size => _icon?.Size ?? new();
        public bool Visible { get; set; } = true;

        private readonly Class _class;
        private readonly Generator<object?> _keyGenerator;
        private readonly IconFactory _iconFactory;
        private readonly IconResolution _resolution;

        private object? _key;
        private Icon? _icon;

        public DynamicIcon(
            Class @class,
            IElementController controller,
            Generator<object?> keyGenerator, 
            IconFactory iconFactory,
            IconResolution resolution)
        {
            _class = @class;
            Controller = controller;
            _keyGenerator = keyGenerator;
            _iconFactory = iconFactory;
            _resolution = resolution;
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            if (_icon != null)
            {
                _icon.OverrideDepth = OverrideDepth;
                _icon.Position = Position;
                _icon.Visible = Visible;
                _icon.Draw(target, context);
            }
        }

        public void Initialize()
        {
            Controller.Bind(this);
        }

        public void Refresh()
        {
            var newKey = _keyGenerator();
            if (newKey != _key)
            {
                if (_icon != null)
                {
                    _icon.Dispose();
                }
                if (newKey != null)
                {
                    _icon = _iconFactory.Create(_class, new InlayController(), newKey, _resolution);
                    _icon.Initialize();
                    _icon.Parent = this;
                }
                else
                {
                    _icon = null;
                }
                _key = newKey;
            }
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public void ResizeContext(Vector3 bounds)
        {
            _icon?.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            _icon?.Update(delta);
        }

        protected override void DisposeImpl()
        {
            _icon?.Dispose();
            _icon = null;
        }
    }
}
