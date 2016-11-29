using System.Windows.Controls;
using System.Windows.Input;
using Prism.Regions;
using Game.AdminClient.ViewModels;

namespace Game.AdminClient.Views
{
    [RegionMemberLifetime(KeepAlive = false)]
    public partial class LobbyView : UserControl
    {
        public LobbyView(LobbyViewModel lobbyViewModel)
        {
            InitializeComponent();

            DataContext = lobbyViewModel;
        }

        private void OnGameDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item == null)
                return;

            ((LobbyViewModel)DataContext).OpenGameCommand.Command.Execute(item.DataContext);
        }
    }
}
