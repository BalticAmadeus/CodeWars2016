using System.Reflection;
using System.Windows;

namespace Game.DebugClient
{
    /// <summary>
    ///     Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow : Window
    {
        public ShellWindow()
        {
            InitializeComponent();

            Title = string.Format("Game Debug Client - ver. {0}", Assembly.GetExecutingAssembly().GetName().Version);
        }
    }
}