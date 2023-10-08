using Cardamom.Collections;
using SpaceOpera.Core.Politics.Diplomacy.Statuses;

namespace SpaceOpera.Core.Politics.Diplomacy.Proposals
{
    public class PeaceProposal : IDiplomaticProposalSection
    {
        public DiplomacyType Type => DiplomacyType.Peace;
        public ISet<DiplomacyType> TypesToCancel => new EnumSet<DiplomacyType>(DiplomacyType.War);
        public bool IsUnilateral => false;

        public void Apply(World world, DiplomaticRelation left, DiplomaticRelation right)
        {
            var status = new AtPeace();
            left.AddStatus(world, status);
            right.AddStatus(world, status);
        }
    }
}
