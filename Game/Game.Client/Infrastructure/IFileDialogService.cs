using System.Threading.Tasks;

namespace Game.AdminClient.Infrastructure
{
    public interface IFileDialogService
    {
        string OpenFileDialog();
        Task<string> OpenFileDialogAsync();
    }
}
