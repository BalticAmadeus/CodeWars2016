namespace Game.AdminClient.Infrastructure
{
    public interface IMessageBoxDialogService
    {
        bool OpenDialog(string message, string title = null);
    }
}
