using Game.AdminClient.Properties;
using Game.AdminClient.Utilities;

namespace Game.AdminClient.Infrastructure
{
    public class SettingsManager : ISettingsManager
    {
        public string ServiceUrl
        {
            get { return Settings.Default.ServiceUrl; }
            set
            {
                Settings.Default.ServiceUrl = value;
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
            get
            {
                string decodedPassword = PasswordEncoder.Decode(Settings.Default.Password);
                return decodedPassword;
            }
            set
            {
                string encodedPassword = PasswordEncoder.Encode(value);
                Settings.Default.Password = encodedPassword;
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