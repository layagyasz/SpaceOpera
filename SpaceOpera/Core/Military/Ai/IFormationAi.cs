﻿using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Military.Ai.Assigments;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.Ai.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai
{
    public interface IFormationAi : ISupplierNode<IAction, FormationContext>
    {
        ICollection<INavigable> GetActiveRegion();
        IAssignment GetAssignment();
        void SetActiveRegion(IEnumerable<INavigable> region);
        void SetAssignment(IAssignment assignment);
    }
}
