using System.Runtime.Serialization;

namespace Game.WebService.Model
{
    [DataContract]
    public class WaitNextTurnReq : BaseReq
    {
        [DataMember]
        public int PlayerId;

        [DataMember]
        public int RefTurn;
    }

    [DataContract]
    public class WaitNextTurnResp : BaseResp
    {
        [DataMember]
        public bool TurnComplete;

        [DataMember]
        public bool GameFinished;

        [DataMember]
        public string FinishCondition;

        [DataMember]
        public string FinishComment;
    }
}
