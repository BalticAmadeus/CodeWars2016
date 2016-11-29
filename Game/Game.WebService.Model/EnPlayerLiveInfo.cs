using GameLogic;
using System;
using System.Runtime.Serialization;

namespace Game.WebService.Model
{
    [DataContract]
    public class EnPlayerLiveInfo
    {
        [DataMember]
        public int PlayerId;

        [DataMember]
        public string Team;

        [DataMember]
        public string Name;

        [DataMember]
        public string Condition;

        [DataMember]
        public string Comment;

        [DataMember]
        public int TurnCompleted;

        [DataMember]
        public DateTime TurnFinTime;

        [DataMember]
        public int PenaltyPoints;

        [DataMember]
        public int BonusPoints;

        [DataMember]
        public int OvertimeTurnMsec;

        [DataMember]
        public int OvertimeTurnTurn;

        [DataMember]
        public int PenaltyThresholdReachedTurn;

        [DataMember]
        public bool SlowTurn { get; set; }

        [DataMember]
        public int Score { get; set; }

        [DataMember]
        public int CurrentDelayMsec;

        public EnPlayerLiveInfo()
        {
            // default
        }

        public EnPlayerLiveInfo(PlayerLiveInfo pi, int gameTurn, DateTime gameTurnStart, DateTime now)
        {
            PlayerId = pi.PlayerId;
            Team = pi.Team.ToString();
            Name = pi.Name;
            Condition = pi.Condition.ToString();
            Comment = pi.Comment;
            TurnCompleted = pi.TurnCompleted;
            TurnFinTime = pi.TurnFinTime;
            PenaltyPoints = pi.PenaltyPoints;
            BonusPoints = pi.BonusPoints;
            OvertimeTurnMsec = pi.OvertimeTurnMsec;
            OvertimeTurnTurn = pi.OvertimeTurnTurn;
            PenaltyThresholdReachedTurn = pi.PenaltyThresholdReachedTurn;
            Score = pi.Score;

            if (TurnCompleted < gameTurn)
            {
                CurrentDelayMsec = (int) (now - gameTurnStart).TotalMilliseconds;
                SlowTurn = CurrentDelayMsec > Settings.SlowTurnIntervalSeconds * 1000;
            }
            else
                CurrentDelayMsec = (int) (TurnFinTime - gameTurnStart).TotalMilliseconds;
        }
    }
}