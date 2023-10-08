using Cardamom.Collections;
using System.Collections.Immutable;

namespace SpaceOpera.Core.Politics.Diplomacy
{
    public class DiplomaticAgreement
    {
        public Faction Proposer { get; }
        public Faction Approver { get; }
        public ImmutableList<IDiplomaticAgreementSection> Left { get; }
        public ImmutableList<IDiplomaticAgreementSection> Right { get; }

        public DiplomaticAgreement(
            Faction proposer,
            Faction approver,
            IEnumerable<IDiplomaticAgreementSection> left,
            IEnumerable<IDiplomaticAgreementSection> right)
        {
            Proposer = proposer;
            Approver = approver;
            Left = left.ToImmutableList();
            Right = right.ToImmutableList();
        }

        public void Apply(World world)
        {
            var typesToCancel = GetTypesToCancel();
            var left = world.DiplomaticRelations.Get(Proposer, Approver);
            var right = world.DiplomaticRelations.Get(Approver, Proposer);

            // Cancel conflicting agreements
            foreach (var agreement in 
                left.CurrentAgreements.Where(x => x.GetTypesToCancel().Intersect(typesToCancel).Count() > 0))
            {
                agreement.Cancel(world);
            }

            // Apply this agreement's effects
            foreach (var section in Left)
            {
                section.Apply(world, left);
            }
            foreach (var section in Right)
            {
                section.Apply(world, right);
            }

            // Notify any existing agreements of the change
            foreach (var relation in world.DiplomaticRelations.GetAsTarget(Proposer).Where(x => x != right))
            {
                foreach (var agreement in relation.CurrentAgreements)
                {
                    agreement.Notify(world, relation, this, /* isProposer= */ true);
                }
            }
            foreach (var relation in world.DiplomaticRelations.GetAsTarget(Approver).Where(x => x != left))
            {
                foreach (var agreement in relation.CurrentAgreements)
                {
                    agreement.Notify(world, relation, this, /* isProposer= */ false);
                }
            }
        }

        public void Cancel(World world)
        {
            var left = world.DiplomaticRelations.Get(Proposer, Approver);
            foreach (var section in Left)
            {
                section.Cancel(world, left);
            }

            var right = world.DiplomaticRelations.Get(Approver, Proposer);
            foreach (var section in Right)
            {
                section.Cancel(world, right);
            }
        }

        public ISet<DiplomacyType> GetTypesToCancel()
        {
            return Left.Concat(Right).SelectMany(x => x.TypesToCancel).ToEnumSet();
        }

        private void Notify(World world, DiplomaticRelation relation, DiplomaticAgreement agreement, bool isProposer)
        {
            foreach (var section in relation.Faction == agreement.Proposer ? Left : Right)
            {
                section.Notify(world, relation, section, isProposer);
            }
        }

        public bool Validate()
        {
            var typesToCancel = GetTypesToCancel();
            foreach (var section in Left)
            {
                // Mirrored proposals must be in both lists.
                if (section.IsMirrored && !Right.Contains(section))
                {
                    return false;
                }
                // Only allow one unilateral declaration at a time.
                if (section.IsUnilateral && Left.Count > 1)
                {
                    return false;
                }
                // Sections cannot cancel each other.
                if (typesToCancel.Contains(section.Type))
                {
                    return false;
                }
            }

            foreach (var section in Right)
            {
                // Mirrored proposals must be in both lists.
                if (section.IsMirrored && !Left.Contains(section))
                {
                    return false;
                }
                // Only allow one unilateral declaration at a time.
                if (section.IsUnilateral && Right.Count > 1)
                {
                    return false;
                }
                // Sections cannot cancel each other.
                if (typesToCancel.Contains(section.Type))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
