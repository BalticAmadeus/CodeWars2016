using Game.DebugClient.Properties;

namespace Game.DebugClient.Infrastructure
{
    public class SettingsManager : ISettingsManager
    {
        public string ServerUrl
        {
            get { return Settings.Default.ServerUrl; }
            set
            {
                Settings.Default.ServerUrl = value;
                Settings.Default.Save();
            }
        }

        public string TeamName
        {
            get { return Settings.Default.TeamName; }
            set
            {
                Settings.Default.TeamName = value;
                Settings.Default.Save();
            }
        }

        public string Password
        {
            get { return Settings.Default.Password; }
            set
            {
                Settings.Default.Password = value;
                Settings.Default.Save();
            }
        }

        public string Username
        {
            get { return Settings.Default.Username; }
            set
            {
                Settings.Default.Username = value;
                Settings.Default.Save();
            }
        }
    }
}