
using System;
namespace BetterGrid
{
    public struct GridLoc
    {
        public readonly int Row;
        public readonly int Col;

        public GridLoc(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public GridLoc Offset(int row, int col)
        {
            return new GridLoc(Row + row, Col + col);
        }

        public override bool Equals(object obj)
        {
            var other = (GridLoc)obj;

            return other.Row == Row && other.Col == Col;
        }

        public override int GetHashCode()
        {
            return Row | (Col << 16);
        }

        public static bool operator!=(GridLoc a, GridLoc b)
        {
            return !a.Equals(b);
        }

        public static bool operator==(GridLoc a, GridLoc b)
        {
            return a.Equals(b);
        }

        public static void Rect(GridLoc a, GridLoc b, out GridLoc upperLeft, out GridLoc bottomRight)
        {
            upperLeft = new GridLoc(Math.Min(a.Row, b.Row), Math.Min(a.Col, b.Col));
            bottomRight = new GridLoc(Math.Max(a.Row, b.Row), Math.Max(a.Col, b.Col));
        }

        public override string ToString()
        {
            return string.Format("GridLoc [{0},{1}]", Row, Col);
        }
    }
}
