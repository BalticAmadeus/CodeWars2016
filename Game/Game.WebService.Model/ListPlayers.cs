using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Game.WebService.Model
{
    [DataContract]
    public class ListPlayersReq : BaseReq
    {
        // empty
    }

    [DataContract]
    public class ListPlayersResp : BaseResp
    {
        [DataMember]
        public List<EnPlayerInfo> Players;
    }
}
