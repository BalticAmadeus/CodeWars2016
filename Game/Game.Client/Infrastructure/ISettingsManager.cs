namespace Game.AdminClient.Infrastructure
{
    public interface ISettingsManager
    {
        string ServiceUrl { get; set; }
        string TeamName { get; set; }
        string Password { get; set; }
        string Username { get; set; }
    }
}
