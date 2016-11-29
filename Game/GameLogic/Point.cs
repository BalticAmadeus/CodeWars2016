using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public struct Point
    {
        public int Row;
        public int Col;

        public Point(int row, int col)
        {
            Row = row;
            Col = col;
        }

        // TODO Equals/GetHashCode is not really necessary for structs

        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                Point other = (Point)obj;
                return other.Row == this.Row && other.Col == this.Col;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Row.GetHashCode() + Col.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", Row, Col);
        }
    }
}
