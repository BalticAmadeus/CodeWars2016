using System.ComponentModel;

namespace Game.DebugClient.Infrastructure
{
    public interface ICommonDataManager : INotifyPropertyChanged
    {
        string TeamName { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        int SessionId { get; set; }
        int SequenceNumber { get; set; }
        string Challenge { get; set; }
        int PlayerId { get; set; }
        string ServerUrl { get; set; }
        int Turn { get; set; }
        bool IAmTecman { get; set; }
        int GameId { get; set; }
    }
}