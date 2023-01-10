using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera
{
    class SerialIdGenerator : IIdGenerator
    {
        private long _CurrentId;

        public SerialIdGenerator(long Start)
        {
            _CurrentId = Start;
        }

        public long Peek()
        {
            return _CurrentId;
        }

        public long Generate()
        {
            return _CurrentId++;
        }
        
        public IIdGenerator Copy()
        {
            return new SerialIdGenerator(_CurrentId);
        }
    }
}