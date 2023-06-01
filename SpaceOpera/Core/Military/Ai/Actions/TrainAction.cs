namespace SpaceOpera.Core.Military.Ai.Actions
{
    public class TrainAction : IAction
    {
        public ActionType Type => ActionType.Train;

        public bool Equivalent(IAction action)
        {
            if (action is RegroupAction)
            {
                return true;
            }
            return false;
        }

        public ActionStatus Progress(AtomicFormationDriver driver, World world)
        {
            return ActionStatus.InProgress;
        }

        public override string ToString()
        {
            return "[TrainAction]";
        }
    }
}

