namespace Game.ClientCommon
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

        public override string ToString()
        {
            return string.Format("({0},{1})", Row, Col);
        }
    }
}
