using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Regions;
using Game.AdminClient.ViewModels;
using Game.AdminClient.Infrastructure;
using GameLogic.UserManagement;

namespace Game.AdminClient.Views
{
    [RegionMemberLifetime(KeepAlive = false)]
    public partial class GamePreviewView : UserControl
    {
        public GamePreviewView(GamePreviewViewModel gamePreviewViewModel)
        {
            InitializeComponent();

            DataContext = gamePreviewViewModel;
        }

        private void GamePreviewViewLoaded(object sender, RoutedEventArgs e)
        {
            //Zoombox.FitToBounds();
            //((GamePreviewViewModel)DataContext).PlayFieldCanvas = PlayFieldCanvas;
        }

        private void OnPlayerDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UserSettings.Role == TeamRole.Observer)
                return;

            var item = sender as ListViewItem;
            if (item == null)
                return;

            ((GamePreviewViewModel)DataContext).DropPlayerCommand.Command.Execute(item.DataContext);
        }
    }
}
