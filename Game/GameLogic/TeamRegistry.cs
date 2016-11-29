using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace GameLogic
{
    public class TeamRegistry
    {
        private Dictionary<string, Team> _teams;
        private string _errorMessage;

        public TeamRegistry()
        {
            _teams = new Dictionary<string, Team>();
            _errorMessage = "Teams not loaded";
        }

        public void Load()
        {
            string fileName = Settings.TeamRegistryFile;
            try
            {
                var xs = new XmlSerializer(typeof(TeamList));
                TeamList tml;
                using (var xr = XmlReader.Create(fileName))
                {
                    tml = (TeamList) xs.Deserialize(xr);
                }
                _teams = tml.Teams.ToDictionary(t => t.Name);
                _errorMessage = string.Format("Team is not registered ({0} known)", _teams.Count);
            }
            catch (Exception ex)
            {
                _errorMessage = string.Format("Error loading registry file '{0}': {1}, {2}", fileName, ex.GetType().FullName, ex.Message);
            }
        }

        public Team GetTeam(string teamName)
        {
            Team team;
            if (_teams.TryGetValue(teamName, out team))
                return team;
            else
                throw new ApplicationException(_errorMessage);
        }
    }

    public class TeamList
    {
        public List<Team> Teams { get; set; }
    }
}
