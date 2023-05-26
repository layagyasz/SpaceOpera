using Cardamom.Mathematics;
using Cardamom.Ui;
using SpaceOpera.Controller.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Components
{
    public class AutoNumericInputTable<T> : BaseNumericInputTable<T> where T : notnull
    {
        public interface IRowConfiguration
        {
            string GetName(T key);
            IntInterval GetRange(T key);
            int GetValue(T key);
        }

        private readonly Func<IEnumerable<T>> _keysFn;
        private readonly IRowConfiguration _configuration;

        public AutoNumericInputTable(
            Style style,
            Func<IEnumerable<T>> keysFn,
            Func<IntInterval> rangeFn,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory,
            IComparer<T> comparer,
            IRowConfiguration configuration)
            : base(
                  new AutoNumericInputTableController<T>("auto-numeric-input-table", rangeFn),
                  style,
                  uiElementFactory,
                  iconFactory,
                  comparer)
        {
            _keysFn = keysFn;
            _configuration = configuration;
        }

        protected override IEnumerable<T> GetKeys()
        {
            return _keysFn();
        }

        protected override NumericInputTableRow<T> CreateRow(T key)
        {
            return NumericInputTableRow<T>.CreateAuto(
                key, _configuration.GetName(key), _uiElementFactory, _iconFactory, _style.Row!.Value, _configuration);
        }
    }
}
