using SpaceOpera.Core.Economics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe
{
    class ResourceNode
    {
        public IMaterial Resource { get; }
        public int Size { get; private set; }

        public ResourceNode(IMaterial Resource, int Size)
        {
            this.Resource = Resource;
            this.Size = Size;
        }

        public void Combine(ResourceNode Other)
        {
            Size += Other.Size;
        }

        public override string ToString()
        {
            return string.Format("[ResourceNode: Resource={0}, Size={1}]", Resource, Size);
        }
    }
}