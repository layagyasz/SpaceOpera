using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Components;

namespace SpaceOpera.View.Panes.FormationPanes
{
    public interface IFormationComponent : IKeyedUiElement<object>
    {
        IController ComponentController { get; }
        UiCompoundComponent Header { get; }
        UiCompoundComponent CompositionTable { get; }
    }
}
