using Cardamom.Ui;

namespace SpaceOpera.View.Components
{
    public interface IKeyedUiElement<T> : IUiElement, IDynamic
    {
        T Key { get; }
    }
}
