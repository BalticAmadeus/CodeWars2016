using System.Windows.Controls;
using Game.DebugClient.ViewModel;

namespace Game.DebugClient.Views
{
    public partial class MainView : UserControl
    {
        public MainView(MainViewModel mainViewModel)
        {
            InitializeComponent();

            DataContext = mainViewModel;
        }
    }
}