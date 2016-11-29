using System.Threading.Tasks;

namespace Game.AdminClient.Infrastructure
{
    public interface IConfirmationDialogService
    {
        bool OpenDialog(string title, string message);
        Task<bool> OpenDialogAsync(string title, string message);
    }
}
