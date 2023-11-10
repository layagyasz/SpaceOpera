namespace SpaceOpera.View.Components.Dynamics
{
    public interface IKeyedElementFactory<T>
    {
        IKeyedUiElement<T> Create(T key);
    }
}
