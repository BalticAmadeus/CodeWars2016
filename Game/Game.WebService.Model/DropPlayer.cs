using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Game.WebService.Model
{
    [DataContract]
    public class DropPlayerReq : BaseReq
    {
        [DataMember]
        public int PlayerId;

        [DataMember]
        public int GameId;
    }

    [DataContract]
    public class DropPlayerResp : BaseResp
    {
        // default
    }
}