using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    /// <summary>
    /// Non-limiting access manager for use in production.
    /// </summary>
    public class OpenAccessManager : IAccessManager
    {
        private AccessToken _permanentToken;

        public OpenAccessManager()
        {
            _permanentToken = new PermanentAccessToken();
        }

        public void CheckAccess(string address)
        {
            // Allow all
        }

        public AccessToken GetAccessToken(string address)
        {
            return _permanentToken;
        }

        public AccessToken GrantAccess(string address, string request)
        {
            return _permanentToken;
        }

        public string RequestAccess(string address)
        {
            return "OPEN";
        }

        public string RequestRevoke(string address)
        {
            return "OPEN";
        }

        public bool RevokeAccess(string address, string request)
        {
            return false;
        }
    }

    public class PermanentAccessToken : AccessToken
    {
        public PermanentAccessToken()
            : base("*")
        {
            MakePermanent();
        }
    }
}
