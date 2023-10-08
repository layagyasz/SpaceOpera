using Cardamom.Collections;
using System.Collections.Immutable;

namespace SpaceOpera.Core.Politics.Diplomacy.Proposals
{
    public class DiplomaticProposal
    {
        public Faction Proposer { get; }
        public Faction Approver { get; }
        public ImmutableList<IDiplomaticProposalSection> Sections { get; }

        public DiplomaticProposal(
            Faction proposer, Faction approver, IEnumerable<IDiplomaticProposalSection> sections)
        {
            Proposer = proposer;
            Approver = approver;
            Sections = sections.ToImmutableList();
        }

        public void Apply(World world)
        {
            var typesToCancel = GetTypesToCancel();
            var left = world.DiplomaticRelations.Get(Proposer, Approver);
            var right = world.DiplomaticRelations.Get(Approver, Proposer);

            left.Cancel(world, typesToCancel);
            right.Cancel(world, typesToCancel);
            foreach (var section in Sections)
            {
                section.Apply(world, left, right);
            }
        }

        public bool Validate()
        {
            // Only allow one unilateral declaration at a time.
            if (Sections.Any(x => x.IsUnilateral) && Sections.Count > 1)
            {
                return false;
            }
            // Sections cannot cancel each other.
            var typesToCancel = GetTypesToCancel();
            if (Sections.Any(x => typesToCancel.Contains(x.Type)))
            {
                return false;
            }
            
            return true;
        }

        private ISet<DiplomacyType> GetTypesToCancel()
        {
            var result = new EnumSet<DiplomacyType>();
            foreach (var section in Sections)
            {
                foreach (var type in section.TypesToCancel)
                {
                    result.Add(type);
                }
            }
            return result;
        }
    }
}
