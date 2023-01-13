using Cardamom.Collections;
using SpaceOpera.Core.Designs;

namespace SpaceOpera.Core.Politics
{
    public class ComponentTagName
    {
        public EnumSet<ComponentTag> Tags { get; }
        public string Name { get; }

        public ComponentTagName(EnumSet<ComponentTag> tags, string name)
        {
            Tags = tags;
            Name = name;
        }
    }
}
