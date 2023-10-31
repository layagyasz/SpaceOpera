using Cardamom.Collections;
using SpaceOpera.Core.Events;

namespace SpaceOpera.Core.Politics.Diplomacy
{
    public class PeaceProposal : IDiplomaticAgreementSection
    {
        public DiplomacyType Type => DiplomacyType.Peace;

        public void Apply(World world, DiplomaticRelation relation)
        {
            relation.SetOverallStatus(DiplomaticRelation.DiplomaticStatus.Peace);
            world.Events.Add(
                new DiplomaticStatusChangeNotification(relation, DiplomaticRelation.DiplomaticStatus.Peace));
        }

        public void Cancel(World world, DiplomaticRelation relation) { }

        public void Notify(
            World world, DiplomaticRelation relation, IDiplomaticAgreementSection agreement, bool isProposer) { }
    }
}
