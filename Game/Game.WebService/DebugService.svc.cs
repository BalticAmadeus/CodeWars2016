using GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Game.WebService
{
    public class DebugService : IDebugService
    {
        protected GameServer Server
        {
            get
            {
                return GameServer.Instance;
            }
        }

        public string CheckMyAccess()
        {
            string clientAddress = ServiceUtils.GetClientAddress();
            AccessToken token = Server.AccessManager.GetAccessToken(clientAddress);
            return DescribeStatus(clientAddress, token);
        }

        private string DescribeStatus(string clientAddress, AccessToken token)
        {
            if (token == null)
                return $"Client {clientAddress} has NO ACCESS";
            if (token.IsValid)
                return $"Client {clientAddress} access GRANTED until {token.ExpirationDate} UTC";
            else
                return $"Client {clientAddress} access REVOKED since {token.ExpirationDate} UTC";
        }

        public string GrantMeAccess(string request)
        {
            string clientAddress = ServiceUtils.GetClientAddress();
            if (string.IsNullOrEmpty(request))
            {
                return Server.AccessManager.RequestAccess(clientAddress);
            }
            else
            {
                AccessToken token = Server.AccessManager.GrantAccess(clientAddress, request);
                return DescribeStatus(clientAddress, token);
            }
        }

        public string RevokeMyAccess(string request)
        {
            string clientAddress = ServiceUtils.GetClientAddress();
            if (string.IsNullOrEmpty(request))
            {
                return Server.AccessManager.RequestRevoke(clientAddress);
            }
            else
            {
                bool revoked = Server.AccessManager.RevokeAccess(clientAddress, request);
                return (revoked) ? $"Access revoked for {clientAddress}"
                                 : $"Access revocation FAILED for Client {clientAddress}";

            }
        }
    }
}
