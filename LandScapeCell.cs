
namespace Problem1
{
    public struct LandScapeCell
    {
        public int X;
        public int Y;
        public int Z;
        public int LengthFromRoot;


        public LandScapeCell(int x, int y, int z, int lengthFromRoot)
        {
            X = x;
            Y = y;
            Z = z;
            LengthFromRoot = lengthFromRoot;
        }
    }

    public struct Cell
    {
        public int X;
        public int Y;
    }
}
