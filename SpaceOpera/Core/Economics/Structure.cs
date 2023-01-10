using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Economics
{
    class Structure : IKeyed
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public uint MaxWorkers { get; set; }
        public uint BuildTime { get; set; }
        public MultiQuantity<IMaterial> Cost { get; set; }

        public override string ToString()
        {
            return string.Format("[Structure: Key={0}]", Key);
        }
    }
}