using Cardamom.Mathematics;
using Cardamom.Trackers;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Components.NumericInputs;

namespace SpaceOpera.Controller.Components.NumericInputs
{
    public class AutoMultiCountInputController<T> : BaseMultiCountInputController<T> where T : notnull
    {
        private readonly Func<IntInterval> _rangeFn;

        public AutoMultiCountInputController(Func<IntInterval> rangeFn)
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
                .Cast<AutoMultiCountInputRowController<T>>()
                .Select(x => new KeyValuePair<T, int>(x.Key, x.GetDelta()))
                .Where(x => x.Value != 0)
                .ToMultiCount(x => x.Key, x => x.Value);
        }

        public void Reset()
        {
            _table!.Refresh();
            ((TableController)_table!.Table.Controller).ResetOffset();
            foreach (var row in _table!.Table.Cast<MultiCountInputRow<T>>())
            {
                ((AutoMultiCountInputRowController<T>)row.ComponentController).Reset();
            }
            UpdateTotal();
        }

        private void HandleRefresh(object? @object, EventArgs e)
        {
            UpdateTotal();
        }
    }
}
