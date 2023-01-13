using SpaceOpera.Core.Designs;

namespace SpaceOpera.Core.Politics
{
    public class NameGeneratorArgs
    {
        public NameType Type { get; }
        public long SequenceNumber { get; set; }
        public string ParentName { get; set; } = string.Empty;
        public List<ComponentTag> Tags { get; set; } = new();

        public NameGeneratorArgs(NameType type)
        {
            Type = type;
        }
    }
}
