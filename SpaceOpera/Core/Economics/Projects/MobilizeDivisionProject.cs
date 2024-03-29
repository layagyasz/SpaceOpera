﻿using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Economics.Projects
{
    public class MobilizeDivisionProject : TimedProject
    {
        public override object Key => Division;
        public override string Name => $"Mobilize {Division.Name}";
        public override Faction Faction => Division.Faction;
        public EconomicSubzoneHolding Holding { get; }
        public Division Division { get; }

        public MobilizeDivisionProject(EconomicSubzoneHolding holding, Division division)
            : base(/* time= */ 10)
        {
            Holding = holding;
            Division = division;
        }

        protected override void CancelImpl()
        {
            Holding.RemoveProject(this);
        }

        public override void Setup()
        {
            Holding.AddProject(this);
        }

        public override void Finish(World world)
        {
            Holding.RemoveProject(this);
            Holding.RemoveDivision(Division);
            Division.SetPosition(Holding.Region.Center);
        }
    }
}
