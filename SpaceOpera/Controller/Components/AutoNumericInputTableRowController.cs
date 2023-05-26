using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class AutoNumericInputTableRowController<T> : BaseNumericInputTableRowController<T> where T : notnull
    {
        private readonly AutoNumericInputTable<T>.IRowConfiguration _configuration;

        private int _defaultValue;

        public AutoNumericInputTableRowController(T key, AutoNumericInputTable<T>.IRowConfiguration configuration)
            : base(key)
        {
            _configuration = configuration;
        }

        public override int GetDefaultValue()
        {
            return _configuration.GetValue(Key);
        }

        public override void Bind(object @object)
        {
            base.Bind(@object);
            _element!.Refreshed += HandleRefresh;
        }

        public override void Unbind()
        {
            _element!.Refreshed -= HandleRefresh;
            base.Unbind();
        }

        public int GetDelta()
        {
            return GetValue() - _configuration.GetValue(Key);
        }

        public void Reset()
        {
            _defaultValue = _configuration.GetValue(Key);
            _inputController!.SetValue(_defaultValue);
            _inputController!.SetRange(_configuration.GetRange(Key));
        }

        private void HandleRefresh(object? sender, EventArgs e)
        {
            if (_inputController!.GetValue() == _defaultValue)
            {
                _defaultValue = _configuration.GetValue(Key);
                _inputController.SetValue(_defaultValue);
            }
            _inputController!.SetRange(_configuration.GetRange(Key));
        }
    }
}
