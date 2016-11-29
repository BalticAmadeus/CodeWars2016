using System.Windows.Controls;
using Game.DebugClient.ViewModel.Flows;

namespace Game.DebugClient.Views.Flows
{
    public partial class CreatePlayerFlowView : UserControl
    {
        public CreatePlayerFlowView(CreatePlayerFlowViewModel createPlayerFlowViewModel)
        {
            InitializeComponent();

            DataContext = createPlayerFlowViewModel;
        }
    }
}