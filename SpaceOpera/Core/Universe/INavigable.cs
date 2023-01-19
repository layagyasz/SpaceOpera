namespace SpaceOpera.Core.Universe
{
    public interface INavigable
    {
        string Name { get; }
        NavigableNodeType NavigableNodeType { get; }
    }
}
