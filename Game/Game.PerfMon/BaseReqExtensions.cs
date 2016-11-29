using Game.PerfMon.AdminService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.PerfMon
{
    public static class BaseReqExtensions
    {
        private static int _sequence;
        private static int _session = new Random().Next();

        public static T Prepare<T>(this T req)
            where T: BaseReq
        {
            req.Auth = new ReqAuth
            {
                TeamName = "Observer",
                ClientName = "PerfMon",
                SessionId = _session,
                SequenceNumber = _sequence++
            };
            var authString = string.Format("{0}:{1}:{2}:{3}{4}",
                req.Auth.TeamName, req.Auth.ClientName, req.Auth.SessionId,
                req.Auth.SequenceNumber, "Observer");
            req.Auth.AuthCode = ClientCommon.Utilites.AuthCodeManager.GetAuthCode(authString);
            return req;
        }
    }
}
