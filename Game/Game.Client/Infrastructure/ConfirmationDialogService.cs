using System.Threading.Tasks;
using System.Windows;

namespace Game.AdminClient.Infrastructure
{
    public class ConfirmationDialogService : IConfirmationDialogService
    {
        public bool OpenDialog(string title, string message)
        {
            var messageBoxResult = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.No);
            return messageBoxResult == MessageBoxResult.Yes;
        }

        public Task<bool> OpenDialogAsync(string title, string message)
        {
            return Task.Run(() => OpenDialog(title, message));
        }
    }
}