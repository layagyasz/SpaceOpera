namespace SpaceOpera
{
    public class ElementEventArgs<T>
    {
        public T Element { get; }

        public ElementEventArgs(T element)
        {
            Element = element;
        }
    }
}
