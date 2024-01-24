namespace SpaceOpera.Core.Designs
{
    public struct ComponentSlot
    {
        public ComponentType Type { get; set; }
        public ComponentSize Size { get; set; }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Size);
        }

        public override bool Equals(object? @object)
        {
            if (@object == null)
            {
                return false;
            }
            if (@object is ComponentSlot other)
            {
                return Type == other.Type && Size == other.Size;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format($"[ComponentSlot: Type={Type}, Size={Size}]");
        }

        public static bool operator ==(ComponentSlot left, ComponentSlot right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ComponentSlot left, ComponentSlot right)
        {
            return !left.Equals(right);
        }
    }
}
