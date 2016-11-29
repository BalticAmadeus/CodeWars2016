using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Game.WebService.Model
{
    [DataContract]
    public class GetTurnResultReq : BaseReq
    {
        [DataMember]
        public int PlayerId;
    }

    [DataContract]
    public class GetTurnResultResp : BaseResp
    {
        [DataMember]
        public int Turn;

        [DataMember]
        public string GameState;

        [DataMember]
        public EnPlayerState[] PlayerStates;
    }
}