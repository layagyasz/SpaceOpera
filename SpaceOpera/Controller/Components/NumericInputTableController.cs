using Cardamom.Ui.Controller;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class NumericInputTableController<T> : IController where T : notnull
    {
        private NumericInputTable<T>? _table;

        public void Bind(object @object)
        {
            _table = (NumericInputTable<T>)@object;
        }

        public void Unbind()
        {
            _table = null;
        }
    }
}
