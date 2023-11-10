using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;

namespace SpaceOpera.View.Components.Dynamics
{
    public class DynamicKeyedTable<T> : UiSerialContainer, IDynamic where T : notnull
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        private readonly Dictionary<T, IKeyedUiElement<T>> _currentRows = new();
        protected readonly IRange<T> _range;
        private readonly IKeyedElementFactory<T> _rowFactory;
        private readonly IComparer<T> _comparer;

        public DynamicKeyedTable(
            Class @class,
            IElementController controller,
            Orientation orientation,
            IRange<T> range,
            IKeyedElementFactory<T> rowFactory,
            IComparer<T> comparer)
            : base(@class, controller, orientation)
        {
            _range = range;
            _rowFactory = rowFactory;
            _comparer = comparer;
        }

        public void Add(T key)
        {
            var element = _rowFactory.Create(key);
            element.Initialize();
            _currentRows.Add(key, element);
            Add(element);
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

            _elements.Sort((x, y) => _comparer.Compare(((IKeyedUiElement<T>)x).Key, ((IKeyedUiElement<T>)y).Key));
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public void Remove(T key)
        {
            var element = _currentRows[key];
            _currentRows.Remove(key);
            Remove(element, /* dispose= */ true);
        }

        public void Reset()
        {
            Clear(true);
            _currentRows.Clear();
            Refresh();
        }

        public bool TryGetRowAs<TOut>(T key, out TOut? row) where TOut : IKeyedUiElement<T>
        {
            return _currentRows.TryGetValueAs(key, out row);
        }
    }
}
