using System.Diagnostics;

namespace Problem1
{
    [DebuggerDisplay("{Z}({X},{Y}), {IsPeak.hasValue ? IsPeak.Value.ToString() : null}")]
    public class LandScapeCell
    {
        public int X;
        public int Y;
        public int Z;
        public bool? IsPeak;
        //public List<LandScapeCell> LongestPath;

        public LandScapeCell()
        {
            
        }

        public LandScapeCell(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

    }

}
