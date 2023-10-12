using Cardamom.Collections;
using System.Collections.Immutable;

namespace SpaceOpera.Core.Politics.Diplomacy
{
    public class DiplomaticAgreement
    {
        public class Builder
        {
            private Faction? _proposer;
            private Faction? _approver;
            private readonly List<IDiplomaticAgreementSection> _left = new();
            private readonly List<IDiplomaticAgreementSection> _right = new();

            public Builder SetProposer(Faction faction)
            {
                _proposer = faction;
                return this;
            }

            public Builder SetApprover(Faction faction)
            {
                _approver = faction;
                return this;
            }

            public Builder AddLeft(IDiplomaticAgreementSection section)
            {
                _left.Add(section);
                return this;
            }

            public Builder AddRight(IDiplomaticAgreementSection section)
            {
                _right.Add(section);
                return this;
            }

            public Builder RemoveLeft(IDiplomaticAgreementSection section)
            {
                _left.Remove(section);
                return this;
            }

            public Builder RemoveRight(IDiplomaticAgreementSection section)
            {
                _right.Remove(section);
                return this;
            }

            public DiplomaticAgreement Build()
            {
                return new(_proposer!, _approver!, _left, _right);
            }
        }

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

        public ISet<DiplomacyType> GetBlocked(Faction faction)
        {
            return GetSections(faction).SelectMany(x => x.TypesToBlock).ToEnumSet();
        }

        public ISet<DiplomacyType> GetCanceled(Faction faction)
        {
            return GetSections(faction).SelectMany(x => x.TypesToCancel).ToEnumSet();
        }

        public bool Validate(World world)
        {
            if (!Validate(Left) || !Validate(Right))
            {
                return false;
            }

            foreach (var section in Left)
            {
                // Mirrored proposals must be in both lists.
                if (section.IsMirrored && !Right.Contains(section))
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
            }

            // Existing agreements prevent this one from being made.
            if (world.DiplomaticRelations.Get(Proposer, Approver).CurrentAgreements.Any(x => x.Blocks(this)))
            {
                return false;
            }
            if (world.DiplomaticRelations.Get(Approver, Proposer).CurrentAgreements.Any(x => x.Blocks(this)))
            {
                return false;
            }

            return true;
        }

        internal void Apply(World world, DiplomaticRelation left, DiplomaticRelation right)
        {
            foreach (var section in Left)
            {
                section.Apply(world, left);
            }
            foreach (var section in Right)
            {
                section.Apply(world, right);
            }
        }

        internal void Cancel(World world)
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

        internal bool Cancels(DiplomaticAgreement other)
        {
            var left = other.GetCanceled(Proposer);
            var right = other.GetCanceled(Approver);
            return Left.Any(x => left.Contains(x.Type)) || Right.Any(x => right.Contains(x.Type));
        }

        internal bool Blocks(DiplomaticAgreement other)
        {
            var left = other.GetBlocked(Proposer);
            var right = other.GetBlocked(Approver);
            return Left.Any(x => left.Contains(x.Type)) || Right.Any(x => right.Contains(x.Type));
        }

        internal void Notify(World world, DiplomaticRelation relation, DiplomaticAgreement agreement, bool isProposer)
        {
            foreach (var section in GetSections(relation.Faction)!)
            {
                foreach (var notification in agreement.GetSections(relation.Target)!)
                {
                    section.Notify(world, relation, notification, isProposer);
                }
            }
        }

        private IEnumerable<IDiplomaticAgreementSection> GetSections(Faction faction)
        {
            if (faction == Proposer)
            {
                return Left;
            }
            if (faction == Approver)
            {
                return Right;
            }
            return Enumerable.Empty<IDiplomaticAgreementSection>();
        }

        private static bool Validate(IList<IDiplomaticAgreementSection> sections)
        {
            var typesToCancel = sections.SelectMany(x => x.TypesToCancel).ToEnumSet();
            var typesToPrevent = sections.SelectMany(x => x.TypesToBlock).ToEnumSet();
            foreach (var section in sections)
            {
                // Only allow one unilateral declaration at a time.
                if (section.IsUnilateral && sections.Count > 1)
                {
                    return false;
                }
                // Sections cannot cancel each other.
                if (typesToCancel.Contains(section.Type))
                {
                    return false;
                }
                // Sections cannot block each other.
                if (typesToPrevent.Contains(section.Type))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
