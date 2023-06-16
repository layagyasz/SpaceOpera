namespace SpaceOpera.Controller.Game
{
    public interface IInterceptor
    {
        EventHandler<EventArgs>? Intercepted { get; set; }

        bool Intercept(UiInteractionEventArgs interaction);
    }
}
