using System.Windows.Controls;
using Game.DebugClient.ViewModel.Flows;

namespace Game.DebugClient.Views.Flows
{
    public partial class WaitNextTurnLoopView : UserControl
    {
        public WaitNextTurnLoopView(WaitNextTurnLoopViewModel waitNextTurnViewModel)
        {
            InitializeComponent();

            DataContext = waitNextTurnViewModel;
        }
    }
}