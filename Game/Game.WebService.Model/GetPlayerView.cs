﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Game.WebService.Model
{
    [DataContract]
    public class GetPlayerViewReq : BaseReq
    {
        [DataMember]
        public int PlayerId;
    }

    [DataContract]
    public class GetPlayerViewResp : BaseResp
    {
        [DataMember]
        public string GameUid;

        [DataMember]
        public int Turn;

        [DataMember]
        public string Mode;

        [DataMember]
        public EnMapData Map;

        [DataMember]
        public EnPoint TecmanPosition;

        [DataMember]
        public List<EnPoint> GhostPositions;

        [DataMember]
        public EnPoint PreviousTecmanPosition;

        [DataMember]
        public List<EnPoint> PreviousGhostPositions;
    }
}
