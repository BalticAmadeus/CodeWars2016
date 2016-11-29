using System.Runtime.Serialization;
using GameLogic;

namespace Game.WebService.Model
{
    [DataContract]
    public class EnGameInfo
    {
        [DataMember]
        public int GameId;

        [DataMember]
        public string Label;

        [DataMember]
        public string State;

        public EnGameInfo()
        {
            // default
        }

        public EnGameInfo(GameInfo gi)
        {
            GameId = gi.GameId;
            Label = gi.Label;
            State = gi.State.ToString();
        }
    }
}
