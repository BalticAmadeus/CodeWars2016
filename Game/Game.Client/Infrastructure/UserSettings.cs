using System.Reflection;
using GameLogic.UserManagement;

namespace Game.AdminClient.Infrastructure
{
    public static class UserSettings
    {
        private const string TitleTemplate = "Admin Client - v{0}";

        public static TeamRole Role { get; set; }

        public static ShellWindow ParentWindow { get; set; }

        public static bool AutoOpen { get; set; }

        public static void SetTitle()
        {
            if (ParentWindow != null)
                ParentWindow.Title = string.Format(TitleTemplate, Assembly.GetExecutingAssembly().GetName().Version);
        }
    }
}
