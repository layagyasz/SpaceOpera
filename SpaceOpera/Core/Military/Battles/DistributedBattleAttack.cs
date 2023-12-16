using Cardamom;

namespace SpaceOpera.Core.Military.Battles
{
    public class DistributedBattleAttack
    {
        private static readonly float s_BaseMultiplier = 0.25f;

        public BattleAttack Attack { get; }

        public float Effort { get; }

        private DistributedBattleAttack(BattleAttack attack, float effort)
        {
            Attack = attack;
            Effort = effort;
        }

        public static DistributedBattleAttack Create(BattleAttack attack, float effort)
        {
            return new DistributedBattleAttack(attack, effort);
        }

        public static List<DistributedBattleAttack> Generate(IEnumerable<BattleAttack> potentials, Random random)
        {
            double[] potential = 
                potentials.Select(
                    x => (double)x.Target.Count.Amount * x.Target.Unit.Threat * x.ComputePotential().GetTotal())
                .ToArray();
            Precondition.Check(potential.All(x => !double.IsNaN(x)));
            double[] dirichlet = Dirichlet.GetDistribution(random, potential);
            Precondition.Check(dirichlet.All(x => !double.IsNaN(x)));
            return potentials.Zip(dirichlet, (x, y) => Create(x, (float)y)).ToList();
        }

        public Damage ComputeRaw()
        {
            return s_BaseMultiplier * Effort * Attack.Attacker.Count.Amount * Attack.ComputeRaw();
        }

        public Damage ComputeOnTarget()
        {
            return s_BaseMultiplier * Effort * Attack.Attacker.Count.Amount * Attack.ComputeOnTarget();
        }

        public Damage ComputeFinal(float adjustment)
        {
            return s_BaseMultiplier * Effort * Attack.Attacker.Count.Amount * Attack.ComputeFinal(adjustment);
        }

        public override string ToString()
        {
            return string.Format("[DistributedBattleAttack: Effort={0}, Attack={1}]", Effort, Attack);
        }
    }
}