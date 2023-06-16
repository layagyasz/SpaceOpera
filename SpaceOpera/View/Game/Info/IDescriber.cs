namespace SpaceOpera.View.Game.Info
{
    public interface IDescriber
    {
        void DescribeAll(IEnumerable<object> objects, InfoPanel infoPanel);
        void Describe(object @object, InfoPanel infoPanel);
    }
}
