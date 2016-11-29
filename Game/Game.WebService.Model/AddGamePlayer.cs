using System.Runtime.Serialization;

namespace Game.WebService.Model
{
    [DataContract]
    public class AddGamePlayerReq : BaseReq
    {
        [DataMember]
        public int GameId;

        [DataMember]
        public int PlayerId;
    }

    [DataContract]
    public class AddGamePlayerResp : BaseResp
    {
        // default
    }
}
