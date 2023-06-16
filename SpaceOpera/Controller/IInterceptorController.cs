using Cardamom.Ui.Controller;

namespace SpaceOpera.Controller
{
    public interface IInterceptorController : IController
    {
        EventHandler<IInterceptor>? InterceptorCreated { get; set; }
        EventHandler<IInterceptor>? InterceptorCancelled { get; set; }
    }
}
