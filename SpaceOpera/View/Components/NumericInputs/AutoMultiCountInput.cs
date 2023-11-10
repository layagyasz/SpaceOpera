using Cardamom.Mathematics;
using Cardamom.Ui;
using SpaceOpera.Controller.Components.NumericInputs;
using SpaceOpera.View.Components.Dynamics;
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

        class ElementFactory : IKeyedElementFactory<T>
        {
            private MultiCountInputStyles.MultiCountInputStyle _style;
            private readonly IRowConfiguration _configuration;
            private readonly UiElementFactory _uiElementFactory;
            private readonly IconFactory _iconFactory;

            public ElementFactory(
                MultiCountInputStyles.MultiCountInputStyle style,
                IRowConfiguration configuration,
                UiElementFactory uiElementFactory,
                IconFactory iconFactory)
            {
                _style = style;
                _uiElementFactory = uiElementFactory;
                _iconFactory = iconFactory;
                _configuration = configuration;
            }

            public IKeyedUiElement<T> Create(T key)
            {
                return MultiCountInputRow<T>.CreateAuto(
                    key, _configuration.GetName(key), _uiElementFactory, _iconFactory, _style.Row!, _configuration);
            }
        }

        public AutoMultiCountInput(
            MultiCountInputStyles.MultiCountInputStyle style,
            IRange<T> range,
            Func<IntInterval> rangeFn,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory,
            IComparer<T> comparer,
            IRowConfiguration configuration)
            : base(
                  new AutoMultiCountInputController<T>(rangeFn),
                  style,
                  uiElementFactory,
                  range,
                  new ElementFactory(style, configuration, uiElementFactory, iconFactory),
                  comparer)
        { }
    }
}
