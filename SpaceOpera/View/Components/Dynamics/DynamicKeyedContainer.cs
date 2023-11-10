using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Components.Dynamics
{
    public class DynamicKeyedContainer<T> : GraphicsResource, IUiContainer, IDynamic where T : notnull
    {
        private class RowComparer : IComparer<IUiElement>
        {
            private readonly IComparer<T> _base;

            public RowComparer(IComparer<T> @base)
            {
                _base = @base;
            }

            public int Compare(IUiElement? x, IUiElement? y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                if (x == null)
                {
                    return -1;
                }
                if (y == null)
                {
                    return 1;
                }
                return _base.Compare(((IKeyedUiElement<T>)x).Key, ((IKeyedUiElement<T>)y).Key);
            }
        }

        public EventHandler<ElementEventArgs>? ElementAdded { get; set; }
        public EventHandler<ElementEventArgs>? ElementRemoved { get; set; }
        public EventHandler<EventArgs>? Refreshed { get; set; }

        public IElementController Controller => _container.Controller;
        public int Count => _container.Count;
        public float? OverrideDepth
        {
            get => _container.OverrideDepth;
            set => _container.OverrideDepth = value;
        }
        public IControlledElement? Parent
        {
            get => _container.Parent;
            set => _container.Parent = value;
        }
        public Vector3 Position
        {
            get => _container.Position;
            set => _container.Position = value;
        }
        public Vector3 Size => _container.Size;
        public bool Visible
        {
            get => _container.Visible;
            set => _container.Visible = value;
        }

        private readonly IUiContainer _container;
        private readonly Dictionary<T, IKeyedUiElement<T>> _currentRows = new();
        protected readonly IRange<T> _range;
        private readonly IKeyedElementFactory<T> _elementFactory;
        private readonly IComparer<IUiElement> _comparer;

        private DynamicKeyedContainer(
            IUiContainer container,
            IRange<T> range,
            IKeyedElementFactory<T> elementFactory,
            IComparer<T> comparer)
        {
            _container = container;
            _range = range;
            _elementFactory = elementFactory;
            _comparer = new RowComparer(comparer);
        }

        public static DynamicKeyedContainer<T> CreateChip(
            Class @class, 
            IElementController controller, 
            IRange<T> range, 
            IKeyedElementFactory<T> elementFactory, 
            IComparer<T> comparer)
        {
            return new(new UiChipContainer(@class, controller), range, elementFactory, comparer);
        }

        public static DynamicKeyedContainer<T> CreateSerial(
            Class @class,
            IElementController controller,
            UiSerialContainer.Orientation orientation,
            IRange<T> range,
            IKeyedElementFactory<T> elementFactory, 
            IComparer<T> comparer)
        {
            return new(new UiSerialContainer(@class, controller, orientation), range, elementFactory, comparer);
        }

        public void Add(IUiElement element)
        {
            throw new NotSupportedException();
        }

        public void Clear(bool dispose)
        {
            throw new NotSupportedException();
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            _container.Draw(target, context);
        }

        public IEnumerator<IUiElement> GetEnumerator()
        {
            return _container.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Initialize()
        {
            _container.Initialize();
            _container.ElementAdded += HandleElementAdded;
            _container.ElementRemoved += HandleElementRemoved;
        }

        public void Insert(int index, IUiElement element)
        {
            throw new NotSupportedException();
        }

        public void Refresh()
        {
            var elements = _range.GetRange().ToHashSet();
            foreach (var element in elements)
            {
                if (!_currentRows.ContainsKey(element))
                {
                    Add(element);
                }
            }
            foreach (var row in _currentRows.Where(x => !elements.Contains(x.Key)).ToList())
            {
                Remove(row.Key);
            }
            foreach (var row in _currentRows.Values)
            {
                row.Refresh();
            }

            _container.Sort(_comparer);
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public void Remove(IUiElement element, bool dispose)
        {
            throw new NotSupportedException();
        }

        public void Remove(T key)
        {
            var element = _currentRows[key];
            _currentRows.Remove(key);
            _container.Remove(element, /* dispose= */ true);
        }

        public void Reset()
        {
            _container.Clear(true);
            _currentRows.Clear();
            Refresh();
        }

        public void ResizeContext(Vector3 bounds)
        {
            _container.ResizeContext(bounds);
        }

        public void Sort(IComparer<IUiElement> comparer)
        {
            throw new NotSupportedException();
        }

        public bool TryGetRowAs<TOut>(T key, out TOut? row) where TOut : IKeyedUiElement<T>
        {
            return _currentRows.TryGetValueAs(key, out row);
        }

        public void Update(long delta)
        {
            _container.Update(delta);
        }

        protected override void DisposeImpl()
        {
            _container.Dispose();
            _container.ElementAdded -= HandleElementAdded;
            _container.ElementRemoved -= HandleElementRemoved;
        }

        private void Add(T key)
        {
            var element = _elementFactory.Create(key);
            element.Initialize();
            _currentRows.Add(key, element);
            _container.Add(element);
        }

        private void HandleElementAdded(object? sender, ElementEventArgs e)
        {
            ElementAdded?.Invoke(this, e);
        }

        private void HandleElementRemoved(object? sender, ElementEventArgs e)
        {
            ElementRemoved?.Invoke(this, e);
        }
    }
}
