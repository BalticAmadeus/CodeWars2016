using System.Runtime.Serialization;

namespace Game.ClientCommon.DataContracts
{
    [DataContract]
    public class CreatePlayerReq : BaseReq
    {
        // empty
    }

    [DataContract]
    public class CreatePlayerResp : BaseResp
    {
        [DataMember]
        public int PlayerId;
    }
}
