using SpaceOpera.Core.Military.Ai;

namespace SpaceOpera.Core.Military
{
    public class DivisionDriver : AtomicFormationDriver
    {
        public DivisionDriver(Division division)
            : base(division, new DivisionAi()) { }
    }
}
