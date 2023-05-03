namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class TrainAction : IAction
    {
        public ActionType Type => ActionType.Train;
        public ActionStatus Status { get; private set; } = ActionStatus.InProgress;

        public bool Equivalent(IAction action)
        {
            if (action is RegroupAction)
            {
                return true;
            }
            return false;
        }

        public void Progress(IFormation formation, World world) { }

        public override string ToString()
        {
            return "[TrainAction]";
        }
    }
}

