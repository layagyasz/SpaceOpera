namespace SpaceOpera.Core.Advanceable
{
    public class ActionTickable : ITickable
    {
        public Action Action { get; }

        public ActionTickable(Action action)
        {
            Action = action;
        }

        public void Tick()
        {
            Action.Invoke();
        }
    }
}
