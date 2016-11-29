using System.Windows.Controls;
using System.Windows.Input;
using Prism.Regions;
using Game.AdminClient.ViewModels;

namespace Game.AdminClient.Views
{
    [RegionMemberLifetime(KeepAlive = false)]
    public partial class OpenGameView : UserControl
    {
        public OpenGameView(OpenGameViewModel openGameViewModel)
        {
            InitializeComponent();

            DataContext = openGameViewModel;
        }

        private void OnAddPlayerDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item == null)
                return;

            ((OpenGameViewModel)DataContext).AddPlayerCommand.Command.Execute(item.DataContext);
        }

        private void OnRemovePlayerDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item == null)
                return;

            ((OpenGameViewModel)DataContext).RemovePlayerCommand.Command.Execute(item.DataContext);
        }
    }
}
