using Game.AdminClient.Infrastructure;
using System;
using System.Reflection;
using System.Windows;

namespace Game.AdminClient
{
    public partial class ShellWindow : Window
    {
        public ShellWindow()
        {
            InitializeComponent();

            UserSettings.ParentWindow = this;
            UserSettings.SetTitle();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}
