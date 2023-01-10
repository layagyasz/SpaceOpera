using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military.Battles
{
    class DistributedBattleAttack
    {
        private static readonly float BASE_MULTIPLIER = 0.25f;

        public BattleAttack Attack { get; }

        public float Effort { get; }

        private DistributedBattleAttack(BattleAttack Attack, float Effort)
        {
            this.Attack = Attack;
            this.Effort = Effort;
        }

        public static DistributedBattleAttack Create(BattleAttack Attack, float Effort)
        {
            return new DistributedBattleAttack(Attack, Effort);
        }

        public static List<DistributedBattleAttack> Generate(IEnumerable<BattleAttack> Potentials, Random Random)
        {
            double[] dirichlet =
                Dirichlet.GetDistribution(
                    Random, 
                    Potentials.Select(
                        x => (double)x.Target.Count * x.Target.Unit.Threat * x.ComputePotential().GetTotal())
                    .ToArray());
            return Potentials.Zip(dirichlet, (x, y) => DistributedBattleAttack.Create(x, (float)y)).ToList();
        }

        public Damage ComputeRaw()
        {
            return BASE_MULTIPLIER * Effort * Attack.Attacker.Count * Attack.ComputeRaw();
        }

        public Damage ComputeOnTarget()
        {
            return BASE_MULTIPLIER * Effort * Attack.Attacker.Count * Attack.ComputeOnTarget();
        }

        public Damage ComputeFinal(float Adjustment)
        {
            return BASE_MULTIPLIER * Effort * Attack.Attacker.Count * Attack.ComputeFinal(Adjustment);
        }

        public override string ToString()
        {
            return string.Format("[DistributedBattleAttack: Effort={0}, Attack={1}]", Effort, Attack);
        }
    }
}