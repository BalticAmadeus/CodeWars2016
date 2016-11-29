using System.Runtime.Serialization;

namespace Game.WebService.Model
{
    [DataContract]
    public class InitLoginReq : BaseReq
    {
        // default
    }

    [DataContract]
    public class InitLoginResp : BaseResp
    {
        [DataMember]
        public string Challenge;
    }
}
