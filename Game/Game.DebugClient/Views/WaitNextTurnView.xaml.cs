using System.Windows.Controls;
using Game.DebugClient.ViewModel;

namespace Game.DebugClient.Views
{
    public partial class WaitNextTurnView : UserControl
    {
        public WaitNextTurnView(WaitNextTurnViewModel waitNextTurnViewModel)
        {
            InitializeComponent();

            DataContext = waitNextTurnViewModel;
        }
    }
}