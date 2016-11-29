using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class ObserverInfo
    {
        public int ObserverId;
        public Team Team;
        public string Name;

        public ObserverInfo(Observer o)
        {
            ObserverId = o.ObserverId;
            Team = o.Team;
            Name = o.Name;
        }
    }
}
