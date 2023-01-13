namespace SpaceOpera
{
    public class SerialIdGenerator : IIdGenerator
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