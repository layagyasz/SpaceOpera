using Cardamom.Collections;
using SpaceOpera.Core.Events;

namespace SpaceOpera.Core.Politics.Diplomacy
{
    public class WarDeclaration : IDiplomaticAgreementSection
    {
        public DiplomacyType Type => DiplomacyType.War;

        public void Apply(World world, DiplomaticRelation relation)
        {
            relation.SetOverallStatus(DiplomaticRelation.DiplomaticStatus.War);
            world.Events.Add(
                new DiplomaticStatusChangeNotification(relation, DiplomaticRelation.DiplomaticStatus.War));
        }

        public void Cancel(World world, DiplomaticRelation relation) { }

        public void Notify(
            World world, DiplomaticRelation relation, IDiplomaticAgreementSection agreement, bool isProposer) { }
    }
}
