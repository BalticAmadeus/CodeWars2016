namespace Game.AdminClient.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string Team { get; set; }
        public string Name { get; set; }
        public int? GameId { get; set; }
    }
}
