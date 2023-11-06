using System.Collections.Immutable;

namespace SpaceOpera.Core.Politics.Diplomacy
{
    public class DiplomaticAgreement
    {
        public record struct RelationTransition(int SetId, bool Origination)
        {
            public static readonly RelationTransition None = new(-1, false);
        }

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
        public RelationTransition LeftTransition { get; }
        public ImmutableList<IDiplomaticAgreementSection> Right { get; }
        public RelationTransition RightTransition { get; }

        public DiplomaticAgreement(
            Faction proposer,
            Faction approver,
            IEnumerable<IDiplomaticAgreementSection> left,
            IEnumerable<IDiplomaticAgreementSection> right)
        {
            Proposer = proposer;
            Approver = approver;
            Left = left.ToImmutableList();
            LeftTransition =
                Left.Aggregate(
                    RelationTransition.None, (x, y) => new(y.Type.SetId, y.Type.IsOriginator | x.Origination));
            Right = right.ToImmutableList();
            RightTransition =
                Right.Aggregate(
                    RelationTransition.None, (x, y) => new(y.Type.SetId, y.Type.IsOriginator | x.Origination));
        }

        public bool Cancels(DiplomaticAgreement other)
        {
            var left = GetTransition(Proposer);
            var right= GetTransition(Approver);
            return (left.Origination && left.SetId != other.GetTransition(Approver).SetId) 
                || (right.Origination && right.SetId != other.GetTransition(Approver).SetId);
        }

        public bool Blocks(DiplomaticAgreement other)
        {
            var left = other.GetTransition(Proposer);
            var right = other.GetTransition(Approver);
            return (!left.Origination && GetTransition(Proposer).SetId != left.SetId)
                || (!right.Origination && GetTransition(Approver).SetId != right.SetId);
        }

        public IEnumerable<IDiplomaticAgreementSection> GetSections(Faction faction)
        {
            if (faction == Proposer)
            {
                return Left;
            }
            if (faction == Approver)
            {
                return Right;
            }
            throw new ArgumentException($"{faction} is not part of this agreement.");
        }

        public RelationTransition GetTransition(Faction faction)
        {
            if (faction == Proposer)
            {
                return LeftTransition;
            }
            if (faction == Approver)
            {
                return RightTransition;
            }
            throw new ArgumentException($"{faction} is not part of this agreement.");
        }

        public Builder ToBuilder()
        {
            var builder = new Builder().SetProposer(Proposer).SetApprover(Approver);
            foreach (var section in Left)
            {
                builder.AddLeft(section);
            }
            foreach (var section in Right)
            {
                builder.AddRight(section);
            }
            return builder;
        }

        public bool Validate(World world)
        {
            if (Left.IsEmpty && Right.IsEmpty)
            {
                return false;
            }
            if (!Validate(world, Left) || !Validate(world, Right))
            {
                return false;
            }

            foreach (var section in Left)
            {
                // Mirrored proposals must be in both lists.
                if (section.Type.IsMirrored && !Right.Contains(section))
                {
                    return false;
                }
            }
            foreach (var section in Right)
            {
                // Mirrored proposals must be in both lists.
                if (section.Type.IsMirrored && !Left.Contains(section))
                {
                    return false;
                }
            }

            // Existing agreements prevent this one from being made.
            var relation = world.DiplomaticRelations.Get(Proposer, Approver);
            if (relation.CurrentAgreements.Any(x => x.Blocks(this)))
            {
                return false;
            }
            foreach (var section in Left)
            {
                if (section.Type.IsUnique)
                {
                    if (relation.CurrentAgreements
                            .SelectMany(x => x.GetSections(Proposer))
                            .Any(x => section.Type == x.Type))
                    {
                        return false;
                    }
                }
            }
            foreach (var section in Right)
            {
                if (section.Type.IsUnique)
                {
                    if (relation.CurrentAgreements
                            .SelectMany(x => x.GetSections(Approver))
                            .Any(x => section.Type == x.Type))
                    {
                        return false;
                    }
                }
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

        private static bool Validate(World world, IList<IDiplomaticAgreementSection> sections)
        {
            var setId = sections.Select(x => x.Type.SetId).FirstOrDefault(-1);
            foreach (var section in sections)
            {
                if (!section.Validate(world))
                {
                    return false;
                }

                // Only allow one unilateral declaration at a time.
                if (section.Type.IsUnilateral && sections.Count > 1)
                {
                    return false;
                }
                // Sections cannot conflict with eachother.
                if (section.Type.SetId != setId)
                {
                    return false;
                }
                // Unique sections must be unique
                if (section.Type.IsUnique && sections.Count(x => x.Type == section.Type) > 1)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
