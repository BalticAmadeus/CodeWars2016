using GameLogic.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameLogic
{
    public class Team
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Secret { get; set; }
        [XmlAttribute]
        public bool Authenticate { get; set; }
        [XmlAttribute]
        public TeamRole Role { get; set; }

        [XmlIgnore]
        public bool PowerTeam => Role == TeamRole.Power;
        [XmlIgnore]
        public bool ObserverTeam => Role == TeamRole.Observer;

        public Team()
        {
            Name = "";
            Secret = "";
            Authenticate = true;
            Role = TeamRole.Normal;
        }

        public override bool Equals(object obj)
        {
            Team other = obj as Team;
            return other != null && other.Name == this.Name;
        }

        public override int GetHashCode()
        {
            return (Name != null) ? Name.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
