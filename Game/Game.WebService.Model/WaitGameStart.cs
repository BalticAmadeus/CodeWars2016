using System.Runtime.Serialization;

namespace Game.WebService.Model
{
    [DataContract]
    public class WaitGameStartReq : BaseReq
    {
        [DataMember]
        public int PlayerId;
    }

    [DataContract]
    public class WaitGameStartResp : BaseResp
    {
        [DataMember]
        public int GameId;
    }
}
