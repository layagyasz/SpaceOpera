using Cardamom.Collections;
using SpaceOpera.Core.Politics.Diplomacy.Statuses;

namespace SpaceOpera.Core.Politics.Diplomacy.Proposals
{
    public class WarDeclaration : IDiplomaticProposalSection
    {
        public DiplomacyType Type => DiplomacyType.War;
        public ISet<DiplomacyType> TypesToCancel =>
            new EnumSet<DiplomacyType>(DiplomacyType.Exchange, DiplomacyType.Pact, DiplomacyType.Peace);
        public bool IsUnilateral => true;

        public void Apply(World world, DiplomaticRelation left, DiplomaticRelation right)
        {
            var status = new AtWar();
            left.SetOverallStatus(DiplomaticRelation.DiplomaticStatus.War);
            right.SetOverallStatus(DiplomaticRelation.DiplomaticStatus.War);
            left.AddStatus(world, status);
            right.AddStatus(world, status);
        }
    }
}
