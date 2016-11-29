namespace GameLogic
{
    public class WaitTurnInfo
    {
        public int PlayerIndex = -1;
        public int Turn;

        public bool TurnComplete;
        public bool GameFinished;
        public PlayerCondition FinishCondition;
        public string FinishComment;
    }
}
