using System.Configuration;
using System.IO;
using System.Web.Hosting;

namespace GameLogic
{
    public static class Settings
    {
        public const int GameStartPollTimeoutMillis = 5000;
        public const int NextTurnPollTimeoutMillis = 1000;
        public const int ObserverPollTimeoutMillis = 2000;
        public const int MapSizeLimit = 100;
        public const int DefaultGameTurnDurationMillis = 500;
        public const int GameTurnDurationAccuracyMillis = 5;
        public const int PenaltyPointsThreshold = 1800; // Total accumulated overtime of 3 min
        public const int TurnResponseThresholdMillis = 1000; // 1 sec
        public const int MinimumSleepMillis = 1;
        public const int MaxObserverQueue = 8;
        public const int LastWaitNextTurnSleepMillis = 1000;
        public const int ReplayDetectionWindowSeconds = 1800; // 30 min
        public const int SlowTurnIntervalSeconds = 3;
        public const int GameTurnLimit = 1000;

        private static string _gameProtocolDir = "Protocols";
        public static string GameProtocolDir { get { return _gameProtocolDir; } }

        private static string _teamRegistryFile = "(not specified)";
        public static string TeamRegistryFile { get { return _teamRegistryFile; } }

        private static bool _isProductionMode = false;
        public static bool IsProductionMode { get { return _isProductionMode; } }

        internal static void Load()
        {
            _gameProtocolDir = ResolvePath(ConfigurationManager.AppSettings["Game.GameProtocolDir"] ?? _gameProtocolDir);
            _teamRegistryFile = ResolvePath(ConfigurationManager.AppSettings["Game.TeamRegistryFile"] ?? _teamRegistryFile);
            _isProductionMode = bool.Parse(ConfigurationManager.AppSettings["Game.ProductionMode"] ?? "False");
        }

        private static string ResolvePath(string path)
        {
            if (path.StartsWith("@"))
                return HostingEnvironment.MapPath(path.Substring(1));
            else
                return path;
        }
    }
}
