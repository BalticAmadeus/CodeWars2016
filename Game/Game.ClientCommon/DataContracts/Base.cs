using System;
using System.Runtime.Serialization;

namespace Game.ClientCommon.DataContracts
{
    [DataContract]
    public class BaseReq
    {
        [DataMember]
        public ReqAuth Auth;
    }

    [DataContract]
    public class ReqAuth
    {
        [DataMember]
        public string TeamName;

        [DataMember]
        public string ClientName;

        [DataMember]
        public int SessionId;

        [DataMember]
        public int SequenceNumber;

        [DataMember]
        public string AuthCode;
    }

    [DataContract]
    public class BaseResp
    {
        [DataMember]
        public string Status;

        [DataMember]
        public string Message;
    }
}
