using System;
using System.Net;

namespace Game.ControlPanel
{
    public class SimpleCredentials : ICredentials
    {
        private NetworkCredential _credentials;

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value;  _credentials = null; }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; _credentials = null; }
        }

        public NetworkCredential GetCredential(Uri uri, string authType)
        {
            if (_credentials == null)
            {
                _credentials = new NetworkCredential(_userName, _password);
            }
            return _credentials;
        }
    }
}