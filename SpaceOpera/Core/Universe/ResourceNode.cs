using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Universe
{
    public class ResourceNode
    {
        public IMaterial Resource { get; }
        public int Size { get; private set; }

        public ResourceNode(IMaterial resource, int size)
        {
            Resource = resource;
            Size = size;
        }

        public void Combine(ResourceNode other)
        {
            Size += other.Size;
        }

        public override string ToString()
        {
            return string.Format("[ResourceNode: Resource={0}, Size={1}]", Resource, Size);
        }
    }
}