using Cardamom.Ui.Controller;

namespace SpaceOpera.Controller.Game
{
    public interface IInterceptorController : IController
    {
        EventHandler<IInterceptor>? InterceptorCreated { get; set; }
        EventHandler<IInterceptor>? InterceptorCancelled { get; set; }
    }
}
