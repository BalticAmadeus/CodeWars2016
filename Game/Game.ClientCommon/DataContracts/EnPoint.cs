using System.Runtime.Serialization;

namespace Game.ClientCommon.DataContracts
{
    [DataContract]
    public class EnPoint
    {
        [DataMember]
        public int Row;

        [DataMember]
        public int Col;

        public EnPoint()
        {
            // default
        }

        public EnPoint(Point p)
        {
            Row = p.Row;
            Col = p.Col;
        }

        public Point ToPoint()
        {
            return new Point(Row, Col);
        }
    }

}
