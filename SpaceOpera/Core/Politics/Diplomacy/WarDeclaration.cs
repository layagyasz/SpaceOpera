using Cardamom.Collections;
using SpaceOpera.Core.Events;

namespace SpaceOpera.Core.Politics.Diplomacy
{
    public class WarDeclaration : IDiplomaticAgreementSection
    {
        public DiplomacyType Type => DiplomacyType.War;
        public ISet<DiplomacyType> TypesToBlock =>
            new EnumSet<DiplomacyType>(DiplomacyType.DefensePact, DiplomacyType.War);
        public ISet<DiplomacyType> TypesToCancel =>
            new EnumSet<DiplomacyType>(DiplomacyType.DefensePact, DiplomacyType.Peace);
        public bool IsMirrored => true;
        public bool IsUnilateral => true;

        public void Apply(World world, DiplomaticRelation relation)
        {
            relation.SetOverallStatus(DiplomaticRelation.DiplomaticStatus.War);
            world.EventManager.Add(
                new DiplomaticStatusChangeNotification(relation, DiplomaticRelation.DiplomaticStatus.War));
        }

        public void Cancel(World world, DiplomaticRelation relation) { }

        public void Notify(
            World world, DiplomaticRelation relation, IDiplomaticAgreementSection agreement, bool isProposer) { }
    }
}
