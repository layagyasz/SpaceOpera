using Adansonia;
using SpaceOpera.Core.Military.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpaceOpera.Core.Military.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Routines
{
    class FleetRoutine
    {
        public static ISupplierNode<IAction, FleetContext> Create()
        {
            return new SelectorNode<AdansoniaNodeResult<IAction>, FleetContext>(
                x => x.Status.Complete, AdansoniaNodeResult<IAction>.Incomplete())
            {
                new CheckContextNode<FleetContext>(x => x.Fleet.Fleet.InCombat)
                    .AndThen(SourceNode<IAction, FleetContext>.Wrap(new CombatAction())),
                new CheckContextNode<FleetContext>(x => !x.Fleet.Fleet.Cohesion.IsFull())
                    .AndThen(SourceNode<IAction, FleetContext>.Wrap(new RegroupAction())),
                PatrolRoutine.Create()
            }.Adapt();
        }
    }
}