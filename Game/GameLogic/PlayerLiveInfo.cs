using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class PlayerLiveInfo
    {
        public int PlayerId;
        public Team Team;
        public string Name;
        public PlayerCondition Condition;
        public string Comment;
        public int TurnCompleted;
        public DateTime TurnFinTime;
        public int PenaltyPoints;
        public int BonusPoints;
        public int OvertimeTurnMsec;
        public int OvertimeTurnTurn;
        public int PenaltyThresholdReachedTurn;
        public int Score { get; set; }
    }
}
