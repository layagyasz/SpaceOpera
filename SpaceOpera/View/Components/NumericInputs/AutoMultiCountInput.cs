using Cardamom.Mathematics;
using Cardamom.Ui;
using SpaceOpera.Controller.Components.NumericInputs;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Components.NumericInputs
{
    public class AutoMultiCountInput<T> : BaseMultiCountInput<T> where T : notnull
    {
        public interface IRowConfiguration
        {
            string GetName(T key);
            IntInterval GetRange(T key);
            int GetValue(T key);
        }

        private readonly Func<IEnumerable<T>> _keysFn;
        private readonly IRowConfiguration _configuration;

        public AutoMultiCountInput(
            MultiCountInputStyles.MultiCountInputStyle style,
            Func<IEnumerable<T>> keysFn,
            Func<IntInterval> rangeFn,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory,
            IComparer<T> comparer,
            IRowConfiguration configuration)
            : base(
                  new AutoMultiCountInputController<T>(rangeFn),
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

        protected override MultiCountInputRow<T> CreateRow(T key)
        {
            return MultiCountInputRow<T>.CreateAuto(
                key, _configuration.GetName(key), _uiElementFactory, _iconFactory, _style.Row!, _configuration);
        }
    }
}
