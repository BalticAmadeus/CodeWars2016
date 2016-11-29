using System.Threading.Tasks;
using Microsoft.Win32;

namespace Game.AdminClient.Infrastructure
{
    public class FileDialogService : IFileDialogService
    {
        public string OpenFileDialog()
        {
            var dialog = new OpenFileDialog();
            bool? result = dialog.ShowDialog();

            return result == true ? dialog.FileName : null;
        }

        public async Task<string> OpenFileDialogAsync()
        {
            return await Task.Run(() => OpenFileDialog());
        }
    }
}