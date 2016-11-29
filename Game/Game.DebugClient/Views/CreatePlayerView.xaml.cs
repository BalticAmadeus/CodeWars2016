using System.Windows.Controls;
using Game.DebugClient.ViewModel;

namespace Game.DebugClient.Views
{
    public partial class CreatePlayerView : UserControl
    {
        public CreatePlayerView(CreatePlayerViewModel initLoginViewModel)
        {
            InitializeComponent();

            DataContext = initLoginViewModel;
        }
    }
}