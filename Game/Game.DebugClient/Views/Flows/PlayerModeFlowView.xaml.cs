using System.Windows.Controls;
using Game.DebugClient.ViewModel.Flows;

namespace Game.DebugClient.Views.Flows
{
    public partial class PlayerModeFlowView : UserControl
    {
        public PlayerModeFlowView(PlayerModeFlowViewModel playerModeFlowViewModel)
        {
            InitializeComponent();

            DataContext = playerModeFlowViewModel;
        }
    }
}