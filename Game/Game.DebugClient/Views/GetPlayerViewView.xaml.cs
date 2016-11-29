using System.Windows.Controls;
using Game.DebugClient.ViewModel;

namespace Game.DebugClient.Views
{
    public partial class GetPlayerViewView : UserControl
    {
        public GetPlayerViewView(GetPlayerViewViewModel getPlayerViewViewModel)
        {
            InitializeComponent();

            DataContext = getPlayerViewViewModel;
        }
    }
}