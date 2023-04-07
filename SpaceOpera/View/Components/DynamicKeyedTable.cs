using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;

namespace SpaceOpera.View.Components
{
    public class DynamicKeyedTable<TKey, TRow> : UiSerialContainer, IDynamic
        where TKey : notnull 
        where TRow : IKeyedUiElement<TKey>
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        private readonly Dictionary<TKey, TRow> _currentRows = new();
        private readonly Func<IEnumerable<TKey>> _rangeFn;
        private readonly Func<TKey, TRow> _rowFn;
        private readonly IComparer<TKey> _comparer;

        public DynamicKeyedTable(
            Class @class, 
            IElementController controller,
            Orientation orientation, 
            Func<IEnumerable<TKey>> rangeFn,
            Func<TKey, TRow> rowFn,
            IComparer<TKey> comparer)
            : base(@class, controller, orientation)
        {
            _rangeFn = rangeFn;
            _rowFn = rowFn;
            _comparer = comparer;
        }

        public void Add(TKey key)
        {
            var element = _rowFn(key);
            element.Initialize();
            _currentRows.Add(key, element);
            Add(element);
        }

        public void Refresh()
        {
            var elements = _rangeFn().ToHashSet();
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

            _elements.Sort((x, y) => _comparer.Compare(((TRow)x).Key, ((TRow)y).Key));
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public void Remove(TKey key)
        {
            var element = _currentRows[key];
            _currentRows.Remove(key);
            Remove(element);
            element.Dispose();
        }
    }
}
