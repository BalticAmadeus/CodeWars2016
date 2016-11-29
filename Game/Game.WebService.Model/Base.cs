using System;
using System.Runtime.Serialization;
using GameLogic;

namespace Game.WebService.Model
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

        public ClientCode GetClientCode()
        {
            return new ClientCode(TeamName, ClientName);
        }
    }

    [DataContract]
    public class BaseResp
    {
        [DataMember]
        public string Status = "OK";

        [DataMember]
        public string Message;
    }

    public static class BaseRespExtensions
    {
        public static T WithException<T>(this T resp, Exception e)
            where T : BaseResp
        {
            if (e is AuthException)
                resp.Status = "AUTH";
            else if (e is WaitException)
                resp.Status = "WAIT";
            else
                resp.Status = "FAIL";
            resp.Message = string.Format("{0}", e.Message);
            return resp;
        }
    }
}
