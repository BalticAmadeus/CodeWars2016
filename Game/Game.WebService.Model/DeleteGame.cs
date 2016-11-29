using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Game.WebService.Model
{
    [DataContract]
    public class DeleteGameReq : BaseReq
    {
        [DataMember]
        public int GameId;
    }

    [DataContract]
    public class DeleteGameResp : BaseResp
    {
        // default
    }
}