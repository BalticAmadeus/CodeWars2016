using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Game.WebService.Model
{
    [DataContract]
    public class StartObservingReq : BaseReq
    {
        [DataMember]
        public int ObserverId;

        [DataMember]
        public int GameId;
    }

    [DataContract]
    public class StartObservingResp : BaseResp
    {
        [DataMember]
        public EnGameDetails GameDetails;

        [DataMember]
        public int Turn;

        [DataMember]
        public EnMapData MapData;

        [DataMember]
        public EnPoint TecmanPosition;

        [DataMember]
        public EnPoint[] GhostPositions;

        [DataMember]
        public EnPlayerState[] PlayerStates;
    }
}