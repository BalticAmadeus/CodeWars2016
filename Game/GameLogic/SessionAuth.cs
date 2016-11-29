using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class SessionAuth
    {
        public string TeamName;
        public string ClientName;
        public int SessionId;
        public int SequenceNumber;
        public string AuthCode;

        public SessionAuth(string teamName, string clientName, int sessionId, int sequenceNumber, string authCode)
        {
            TeamName = teamName;
            ClientName = clientName;
            SessionId = sessionId;
            SequenceNumber = sequenceNumber;
            AuthCode = authCode;
        }

        public ClientCode GetClientCode()
        {
            return new ClientCode(TeamName, ClientName);
        }

        public string GetAuthString()
        {
            return string.Format("{0}:{1}:{2}:{3}", TeamName, ClientName, SessionId, SequenceNumber);
        }
    }

    public class SessionAuthOptions
    {
        public bool IsLoginFlow = false;
    }
}