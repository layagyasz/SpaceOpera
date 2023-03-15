namespace SpaceOpera.View.Info
{
    public interface IDescriber
    {
        void Describe(IEnumerable<object> objects, InfoPanel infoPanel);
        void Describe(object @object, InfoPanel infoPanel);
    }
}
