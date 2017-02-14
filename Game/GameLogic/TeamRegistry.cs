using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace GameLogic
{
    public class TeamRegistry
    {
        private object _reloadLock;
        private Dictionary<string, Team> _teams;
        private DateTime _loadedChangeTime;
        private DateTime _lastCheckTime;
        private string _errorMessage;

        public TeamRegistry()
        {
            _reloadLock = new object();
            _teams = new Dictionary<string, Team>();
            _errorMessage = "Teams not loaded";
        }

        private bool IsReloadNecessary()
        {
            DateTime now = DateTime.UtcNow;
            if (_lastCheckTime.AddSeconds(Settings.RegistryReloadIntervalSeconds) > now)
                return false;
            _lastCheckTime = now;
            try
            {
                DateTime changeTime = File.GetLastWriteTimeUtc(Settings.TeamRegistryFile);
                if (changeTime == _loadedChangeTime)
                    return false;
            }
            catch (Exception ex)
            {
                _errorMessage = $"Problem with registry file '{Settings.TeamRegistryFile}' @ {_lastCheckTime}: {ex.GetType().FullName}, {ex.Message}";
                return false;
            }
            return true;
        }

        private void Reload()
        {
            string fileName = Settings.TeamRegistryFile;
            DateTime changeTime = File.GetLastWriteTimeUtc(fileName);
            lock (_reloadLock)
            {
                if (_loadedChangeTime == changeTime)
                    return;
                _loadedChangeTime = changeTime;
                try
                {
                    var xs = new XmlSerializer(typeof(TeamList));
                    TeamList tml;
                    using (var xr = XmlReader.Create(fileName))
                    {
                        tml = (TeamList)xs.Deserialize(xr);
                    }
                    _teams = tml.Teams.ToDictionary(t => t.Name);
                    _errorMessage = string.Format("Team is not registered ({0} known)", _teams.Count);
                }
                catch (Exception ex)
                {
                    _errorMessage = $"Error loading registry file '{fileName}' @ {_loadedChangeTime}: {ex.GetType().FullName}, {ex.Message}";
                }
            }
        }

        public Team GetTeam(string teamName)
        {
            if (IsReloadNecessary())
                Reload();
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
