using System.Runtime.Serialization;

namespace Game.WebService.Model
{
    [DataContract]
    public class StartGameReq : BaseReq
    {
        [DataMember]
        public int GameId;
    }

    [DataContract]
    public class StartGameResp : BaseResp
    {
        // default
    }
}
