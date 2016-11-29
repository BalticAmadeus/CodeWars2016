namespace Game.DebugClient.Infrastructure
{
    public interface ISettingsManager
    {
        string ServerUrl { get; set; }
        string TeamName { get; set; }
        string Password { get; set; }
        string Username { get; set; }
    }
}