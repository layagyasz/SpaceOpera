namespace SpaceOpera.Core.Military.Ai.Actions
{
    public interface IAction
    {
        ActionType Type { get; }
        bool Equivalent(IAction action);
        ActionStatus Progress(AtomicFormationDriver driver, World world);
    }
}
