using GameLogic;
using System.Runtime.Serialization;

namespace Game.WebService.Model
{
    [DataContract]
    public class EnPlayerState
    {
        [DataMember]
        public string Condition;

        [DataMember]
        public string Comment;

        [DataMember]
        public string Mode;

        [DataMember]
        public int Score { get; set; }

        public EnPlayerState()
        {
            //default
        }

        public EnPlayerState(PlayerStateInfo ps)
        {
            Condition = ps.Condition.ToString();
            Comment = ps.Comment;
            Mode = ps.Mode.ToString();
            Score = ps.Score;
        }
    }
}