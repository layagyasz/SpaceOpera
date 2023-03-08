using Cardamom.Ui;

namespace SpaceOpera.View.Components
{
    public interface IActionRow
    {
        IEnumerable<IUiElement> GetActions();
    }
}
