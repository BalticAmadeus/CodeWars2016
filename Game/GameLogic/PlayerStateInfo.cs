using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    public class PlayerStateInfo
    {
        public PlayerCondition Condition;
        public PlayerMode Mode;
        public string Comment;
        public int Score;

        public PlayerStateInfo(PlayerState ps)
        {
            Condition = ps.Condition;
            Mode = ps.Mode;
            Comment = ps.Comment;
            Score = ps.Score;
        }
    }
}
