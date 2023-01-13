namespace SpaceOpera.Core.Military.Actions
{
    public interface IAction
    {
        bool Equivalent(IAction action);
        void Progress(IFormation formation, World world);
    }
}
