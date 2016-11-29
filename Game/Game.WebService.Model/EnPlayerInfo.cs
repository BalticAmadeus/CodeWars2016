using System.Runtime.Serialization;
using GameLogic;

namespace Game.WebService.Model
{
    [DataContract]
    public class EnPlayerInfo
    {
        [DataMember]
        public int PlayerId;

        [DataMember]
        public string Team;

        [DataMember]
        public string Name;

        [DataMember]
        public int? GameId;

        public EnPlayerInfo()
        {
            // default
        }

        public EnPlayerInfo(PlayerInfo pi)
        {
            PlayerId = pi.PlayerId;
            Team = pi.Team.Name;
            Name = pi.Name;
            GameId = pi.GameId;
        }
    }
}
