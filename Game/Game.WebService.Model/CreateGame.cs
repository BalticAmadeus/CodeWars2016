using System.Runtime.Serialization;

namespace Game.WebService.Model
{
    [DataContract]
    public class CreateGameReq : BaseReq
    {
        // empty
    }

    [DataContract]
    public class CreateGameResp : BaseResp
    {
        [DataMember]
        public EnGameInfo GameInfo;
    }
}
