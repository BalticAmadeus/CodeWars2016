using System.Windows.Controls;
using Game.DebugClient.ViewModel;

namespace Game.DebugClient.Views
{
    public partial class CommonDataView : UserControl
    {
        public CommonDataView(CommonDataViewModel commonDataViewModel)
        {
            InitializeComponent();

            DataContext = commonDataViewModel;
        }
    }
}