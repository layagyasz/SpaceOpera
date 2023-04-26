namespace SpaceOpera.Core.Military.Ai.Actions
{
    public interface IAction
    {
        ActionStatus Status { get; }
        bool Equivalent(IAction action);
        void Progress(IFormation formation, World world);
    }
}
