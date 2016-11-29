using System.Windows.Controls;
using Game.DebugClient.ViewModel;

namespace Game.DebugClient.Views
{
    public partial class LoggerView : UserControl
    {
        public LoggerView(LoggerViewModel loggerViewModel)
        {
            InitializeComponent();

            DataContext = loggerViewModel;
        }
    }
}