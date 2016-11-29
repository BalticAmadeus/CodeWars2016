using System.Windows;

namespace Game.AdminClient.Infrastructure
{
    public class MessageBoxDialogService : IMessageBoxDialogService
    {
        public bool OpenDialog(string message, string title = null)
        {
            var messageBoxResult = MessageBox.Show(message, title, MessageBoxButton.OK);
            return messageBoxResult == MessageBoxResult.OK;
        }
    }
}