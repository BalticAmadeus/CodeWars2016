using GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Game.WebService.Model
{
    [DataContract]
    public class EnObsGameInfo
    {
        [DataMember]
        public int GameId;

        [DataMember]
        public string GameState;

        [DataMember]
        public int QueuedTurns;

        [DataMember]
        public List<int> SlowPlayers;

        public EnObsGameInfo()
        {
            // default
        }

        public EnObsGameInfo(ObservedGameInfo gi)
        {
            GameId = gi.GameId;
            GameState = gi.GameState.ToString();
            QueuedTurns = gi.QueuedTurns;
            SlowPlayers = gi.SlowPlayers;
        }
    }
}