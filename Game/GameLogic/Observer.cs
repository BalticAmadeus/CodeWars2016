using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class Observer
    {
        public int ObserverId { get; private set; }
        public Team Team { get; private set; }
        public string Name { get; private set; }

        public Observer(int observerId, Team team, string name)
        {
            ObserverId = observerId;
            Team = team;
            Name = name;
        }
    }
}
