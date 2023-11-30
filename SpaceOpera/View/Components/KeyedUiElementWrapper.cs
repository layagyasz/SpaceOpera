using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Components
{
    public class KeyedUiElementWrapper<T> : GraphicsResource, IKeyedUiElement<T>
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        public T Key { get; }

        private readonly IUiElement _element;

        public IElementController Controller => _element.Controller;
        public IControlledElement? Parent
        {
            get => _element.Parent;
            set => _element.Parent = value;
        }
        public float? OverrideDepth
        {
            get => _element.OverrideDepth;
            set => _element.OverrideDepth = value;
        }
        public Vector3 Position
        {
            get => _element.Position;
            set => _element.Position = value;
        }
        public Vector3 Size
        {
            get => _element.Size;
            set => _element.Position = value;
        }
        public bool Visible
        {
            get => _element.Visible;
            set => _element.Visible = value;
        }

        private KeyedUiElementWrapper(T key, IUiElement element)
        {
            Key = key;
            _element = element;
        }

        public static KeyedUiElementWrapper<T> Wrap(T key, IUiElement element)
        {
            return new(key, element);
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            _element.Draw(target, context);
        }

        public void Initialize()
        {
            _element.Initialize();
        }

        public void Refresh()
        {
            if (_element is IDynamic dynamic)
            {
                dynamic.Refresh();
            }
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public void ResizeContext(Vector3 bounds)
        {
            _element.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            _element.Update(delta);
        }

        protected override void DisposeImpl()
        {
            _element.Dispose();
        }
    }
}
