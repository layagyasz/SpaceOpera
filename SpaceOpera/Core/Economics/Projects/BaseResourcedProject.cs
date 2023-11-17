﻿using Cardamom.Trackers;

namespace SpaceOpera.Core.Economics.Projects
{
    public abstract class BaseResourcedProject : BaseProject
    {
        public EconomicSubzoneHolding Holding { get; }
        public float Time { get; }
        public override Pool Progress { get; }
        public MultiQuantity<IMaterial> Cost { get; }

        protected BaseResourcedProject(EconomicSubzoneHolding holding, float time, MultiQuantity<IMaterial> cost) 
        {
            Holding = holding;
            Time = time;
            Progress = new Pool(time, /* startFull= */ false);
            Cost = cost;
        }

        public override void Setup()
        {
            Holding.AddProject(this);
        }

        public override void Finish(World world)
        {
            Holding.RemoveProject(this);
        }

        protected override void CancelImpl()
        {
            Holding.Parent.Return(Progress.PercentFull() * Cost);
        }

        protected override void TickImpl()
        {
            var progress = Holding.Parent.Spend(Cost, 1f / Progress.MaxAmount);
            Progress.Change(progress * Time);
            Status = progress > float.Epsilon ? ProjectStatus.InProgress : ProjectStatus.Blocked;
        }
    }
}
