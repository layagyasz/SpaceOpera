namespace SpaceOpera
{
    public struct ValueEventArgs<T>
    {
        public T Element { get; }

        public ValueEventArgs(T element)
        {
            Element = element;
        }

        public override string ToString()
        {
            return $"[ElementEventArgs: Element={Element}]";
        }
    }
}
