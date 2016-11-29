using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameLogic
{
    public struct ClientCode
    {
        public string TeamName { get; private set; }
        public string ClientName { get; private set; }

        public ClientCode(string teamName, string clientName) : this()
        {
            TeamName = teamName;
            ClientName = clientName;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ClientCode))
                return false;
            ClientCode other = (ClientCode)obj;
            return this.TeamName == other.TeamName
                && this.ClientName == other.ClientName;
        }

        public override int GetHashCode()
        {
            return ((TeamName == null) ? 0 : TeamName.GetHashCode())
                + ((ClientName == null) ? 0 : ClientName.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", TeamName, ClientName);
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ClientName))
                throw new ApplicationException("ClientName is empty");
            if (ClientName.Length > 30)
                throw new ApplicationException("ClientName is too long");
            if (ClientName.Trim() != ClientName)
                throw new ApplicationException("ClientName has leading or trailing whitespace");
            if (!Regex.IsMatch(ClientName, "^[ -~]*$"))
                throw new ApplicationException("ClientName contains invalid characters");
        }
    }
}
