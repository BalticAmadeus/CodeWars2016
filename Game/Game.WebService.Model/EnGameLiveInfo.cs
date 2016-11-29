using GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Game.WebService.Model
{
    [DataContract]
    public class EnGameLiveInfo
    {
        [DataMember]
        public string GameState;
        [DataMember]
        public int Turn;
        [DataMember]
        public DateTime TurnStartTime;
        [DataMember]
        public EnPlayerLiveInfo[] PlayerStates;

        public EnGameLiveInfo()
        {
            // default
        }

        public EnGameLiveInfo(GameLiveInfo gi)
        {
            GameState = gi.GameState.ToString();
            Turn = gi.Turn;
            TurnStartTime = gi.TurnStartTime;
            DateTime now = DateTime.Now;
            PlayerStates = gi.PlayerStates.Select(p => new EnPlayerLiveInfo(p, Turn, TurnStartTime, now)).ToArray();
        }
    }
}