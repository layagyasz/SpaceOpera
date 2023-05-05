﻿namespace SpaceOpera.Core.Military.Ai.Actions
{
    public interface IAction
    {
        ActionType Type { get; }
        ActionStatus Status { get; }
        bool Equivalent(IAction action);
        void Progress(IAtomicFormation formation, World world);
    }
}
