﻿namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class CombatAction : IAction
    {
        public ActionType Type => ActionType.Combat;

        public bool Equivalent(IAction action)
        {
            if (action is CombatAction)
            {
                return true;
            }
            return false;
        }

        public ActionStatus Progress(AtomicFormationDriver driver, World world)
        {
            return driver.AtomicFormation.InCombat == 0 ? ActionStatus.Done : ActionStatus.InProgress;
        }
    }
}
