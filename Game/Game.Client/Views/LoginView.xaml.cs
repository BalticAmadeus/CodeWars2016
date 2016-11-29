using System.Windows.Controls;
using Prism.Regions;
using Game.AdminClient.ViewModels;

namespace Game.AdminClient.Views
{
    [RegionMemberLifetime(KeepAlive = false)]
    public partial class LoginView : UserControl
    {
        public LoginView(LoginViewModel loginViewModel)
        {
            InitializeComponent();

            DataContext = loginViewModel;
        }
    }
}
