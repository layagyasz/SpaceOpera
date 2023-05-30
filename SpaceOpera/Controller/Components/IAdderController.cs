using Cardamom.Ui.Controller;

namespace SpaceOpera.Controller.Components
{
    public interface IAdderController<T> : IController
    {
        EventHandler<T>? Added { get; set; }
    }
}
