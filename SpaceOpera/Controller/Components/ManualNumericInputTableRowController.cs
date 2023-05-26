namespace SpaceOpera.Controller.Components
{
    public class ManualNumericInputTableRowController<T> : BaseNumericInputTableRowController<T> where T : notnull
    {
        public ManualNumericInputTableRowController(T key)
            : base(key) { }

        public override int GetDefaultValue()
        {
            return 0;
        }
    }
}
