using Cardamom.Mathematics;
using Cardamom.Trackers;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class SyncingNumericInputTableController<T> : BaseNumericInputTableController<T> where T : notnull
    {
        private readonly Func<IntInterval> _rangeFn;

        public SyncingNumericInputTableController(Func<IntInterval> rangeFn)
        {
            _rangeFn = rangeFn;
        }

        public override IntInterval GetRange()
        {
            return _rangeFn();
        }

        public override void Bind(object @object)
        {
            base.Bind(@object);
            _table!.Refreshed += HandleRefresh;
        }

        public override void Unbind()
        {
            _table!.Refreshed -= HandleRefresh;
            _table = null;
            base.Unbind();
        }

        public MultiCount<T> GetDeltas()
        {
            return _table!.Table
                .Select(x => ((UiCompoundComponent)x).ComponentController)
                .Cast<SyncingNumericInputTableRowController<T>>()
                .Select(x => new KeyValuePair<T, int>(x.Key, x.GetDelta()))
                .Where(x => x.Value != 0)
                .ToMultiCount(x => x.Key, x => x.Value);
        }

        public void Reset()
        {
            _table!.Refresh();
            ((TableController)_table!.Table.Controller).ResetOffset();
            foreach (var row in _table!.Table.Cast<NumericInputTableRow<T>>())
            {
                ((SyncingNumericInputTableRowController<T>)row.ComponentController).Reset();
            }
            UpdateTotal();
        }

        private void HandleRefresh(object? @object, EventArgs e)
        {
            UpdateTotal();
        }
    }
}
