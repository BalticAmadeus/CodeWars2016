using System.Runtime.Serialization;

namespace Game.WebService.Model
{
    [DataContract]
    public class RemoveGamePlayerReq : BaseReq
    {
        [DataMember]
        public int GameId;

        [DataMember]
        public int PlayerId;
    }

    [DataContract]
    public class RemoveGamePlayerResp : BaseResp
    {
        // default
    }
}
