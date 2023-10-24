using SpaceOpera.Core.Events;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.Core.Orders
{
    public class ProposeDiplomaticAgreementOrder : IOrder
    {
        public DiplomaticAgreement Agreement { get; }

        public ProposeDiplomaticAgreementOrder(DiplomaticAgreement agreement)
        {
            Agreement = agreement;
        }

        public ValidationFailureReason Validate(World world)
        {
            return Agreement.Validate(world) ? ValidationFailureReason.None : ValidationFailureReason.IllegalOrder;
        }

        public bool Execute(World world)
        {
            world.EventManager.Add(new ProposeDiplomaticAgreementEvent(Agreement));
            return true;
        }
    }
}
