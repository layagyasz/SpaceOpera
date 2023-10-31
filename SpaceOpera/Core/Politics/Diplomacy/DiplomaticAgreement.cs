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

        public bool Cancels(DiplomaticAgreement other)
        {
            var left = GetSetId(Proposer);
            var right= GetSetId(Approver);
            return (left.Item2 && left.Item1 != other.GetSetId(Approver).Item1) 
                || (right.Item2 && right.Item1 != other.GetSetId(Approver).Item1);
        }

        public bool Blocks(DiplomaticAgreement other)
        {
            var left = other.GetSetId(Proposer);
            var right = other.GetSetId(Approver);
            return (!left.Item2 && GetSetId(Proposer).Item1 != left.Item1)
                || (!right.Item2 && GetSetId(Approver).Item1 != right.Item1);
        }

        public IEnumerable<IDiplomaticAgreementSection> GetSections(bool isLeft)
        {
            return isLeft ? Left : Right;
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

        public (int, bool) GetSetId(Faction faction)
        {
            int setId = -1;
            bool originate = false;
            foreach (var section in GetSections(faction))
            {
                setId = section.Type.SetId;
                originate = section.Type.IsOriginator;
            }
            return (setId, originate);
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
            if (!Validate(Left) || !Validate(Right))
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

        private static bool Validate(IList<IDiplomaticAgreementSection> sections)
        {
            var setId = sections.Select(x => x.Type.SetId).FirstOrDefault(-1);
            foreach (var section in sections)
            {
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
