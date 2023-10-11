using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Components
{
    public class KeyedUiComponent<T> : GraphicsResource, IKeyedUiElement<T>, IUiComponent
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        public T Key { get; }
        public IController ComponentController => _component.ComponentController;
        public IElementController Controller => _component.Controller;
        public IControlledElement? Parent
        {
            get => _component!.Parent;
            set => _component!.Parent = value;
        }
        public Vector3 Position
        {
            get => _component!.Position;
            set => _component!.Position = value;
        }

        public Vector3 Size => _component!.Size;
        public bool Visible
        {
            get => _component!.Visible;
            set => _component!.Visible = value;
        }
        public float? OverrideDepth
        {
            get => _component!.OverrideDepth;
            set => _component!.OverrideDepth = value;
        }

        private readonly IUiComponent _component;

        private KeyedUiComponent(T key, IUiComponent component)
        {
            Key = key;
            _component = component;
        }

        public static KeyedUiComponent<T> Wrap(T key, IUiComponent component)
        {
            return new(key, component);
        }

        protected override void DisposeImpl()
        {
            _component!.Dispose();
        }

        public virtual void Draw(IRenderTarget target, IUiContext context)
        {
            _component!.Draw(target, context);
        }

        public IUiComponent GetComponent()
        {
            return _component!;
        }

        public void Initialize()
        {
            _component?.Initialize();
            if (_component is IDynamic dynamic)
            {
                dynamic.Refreshed += HandleRefresh;
            }
        }

        public void Refresh()
        {
            if (_component is IDynamic dynamic)
            {
                dynamic.Refresh();
            }
        }

        public virtual void ResizeContext(Vector3 bounds)
        {
            _component!.ResizeContext(bounds);
        }

        public void Update(long delta)
        {
            _component!.Update(delta);
        }

        private void HandleRefresh(object? sender, EventArgs e)
        {
            Refreshed?.Invoke(this, e);
        }
    }
}
