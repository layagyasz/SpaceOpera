namespace SpaceOpera.Controller
{
    public interface IValueInterceptor<T> : IInterceptor
    {
        T Get();
    }
}
