using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GameLogic;

namespace Game.WebService.Model
{
    [DataContract]
    public class EnGameDetails
    {
        [DataMember]
        public int GameId;

        [DataMember]
        public string Label;

        [DataMember]
        public string State;

        [DataMember]
        public List<EnPlayerInfo> Players;

        public EnGameDetails()
        {
            // default
        }

        public EnGameDetails(GameDetails gd)
        {
            GameId = gd.GameId;
            Label = gd.Label;
            State = gd.State.ToString();
            Players = gd.Players.Select(p => new EnPlayerInfo(p)).ToList();
        }
    }
}
