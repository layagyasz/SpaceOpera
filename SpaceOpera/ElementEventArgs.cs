namespace SpaceOpera
{
    public struct ElementEventArgs<T>
    {
        public T Element { get; }

        public ElementEventArgs(T element)
        {
            Element = element;
        }

        public override string ToString()
        {
            return $"[ElementEventArgs: Element={Element}]";
        }
    }
}
