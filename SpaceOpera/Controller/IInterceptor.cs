namespace SpaceOpera.Controller
{
    public interface IInterceptor
    {
        EventHandler<EventArgs>? Intercepted { get; set; }

        bool Intercept(UiInteractionEventArgs interaction);
    }
}
