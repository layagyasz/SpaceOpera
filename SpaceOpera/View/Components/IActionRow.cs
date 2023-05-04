using Cardamom.Ui;

namespace SpaceOpera.View.Components
{
    public interface IActionRow : IUiElement
    {
        EventHandler<ElementEventArgs>? ActionAdded { get; set; }
        EventHandler<ElementEventArgs>? ActionRemoved { get; set; }
        IEnumerable<IUiElement> GetActions();
    }
}
