using System.Windows.Controls;
using Game.DebugClient.ViewModel;

namespace Game.DebugClient.Views
{
    public partial class PerformMoveView : UserControl
    {
        public PerformMoveView(PerformMoveViewModel performMoveViewModel)
        {
            InitializeComponent();

            DataContext = performMoveViewModel;
        }
    }
}