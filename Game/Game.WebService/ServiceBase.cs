using System;
using GameLogic;
using Game.WebService.Model;
using System.ServiceModel.Web;

namespace Game.WebService
{
    public class ServiceBase
    {
        protected GameServer Server
        {
            get
            {
                return GameServer.Instance;
            }
        }

        protected TResp HandleServiceCall<TReq, TResp>(TReq req, TResp resp, Action<TReq, TResp> handler, SessionAuthOptions authOptions = null)
            where TReq : BaseReq
            where TResp : BaseResp, new()
        {
            try
            {
                string clientAddress = ServiceUtils.GetClientAddress();
                Server.AccessManager.CheckAccess(clientAddress);
            }
            catch (AuthException)
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            try
            {
                if (authOptions == null)
                    authOptions = new SessionAuthOptions();
                var sessionAuth = GetSessionAuth(req);
                var sessionManager = Server.SessionManager;
                sessionManager.Authenticate(sessionAuth, authOptions);
                handler(req, resp);
                return resp;
            }
            catch (Exception e)
            {
                //log.Error("HandleServiceCall top-level exception handler", e);
                return new TResp().WithException(e);
            }
        }

        private SessionAuth GetSessionAuth(BaseReq req)
        {
            if (req == null || req.Auth == null)
                return new SessionAuth("(no team)", "(anonymous)", 0, 0, "");
            ReqAuth a = req.Auth;
            return new SessionAuth(a.TeamName, a.ClientName, a.SessionId, a.SequenceNumber, a.AuthCode);
        }
    }
}
