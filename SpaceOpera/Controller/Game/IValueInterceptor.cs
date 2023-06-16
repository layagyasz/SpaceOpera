namespace SpaceOpera.Controller.Game
{
    public interface IValueInterceptor<T> : IInterceptor
    {
        T Get();
    }
}
