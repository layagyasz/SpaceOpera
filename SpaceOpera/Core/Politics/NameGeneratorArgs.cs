using SpaceOpera.Core.Designs;

namespace SpaceOpera.Core.Politics
{
    public class NameGeneratorArgs
    {
        public NameType Type { get; }
        public long SequenceNumber { get; set; }
        public string ParentName { get; set; } = string.Empty;
        public ComponentTag[] Tags { get; set; } = Array.Empty<ComponentTag>();

        public NameGeneratorArgs(NameType type)
        {
            Type = type;
        }
    }
}
