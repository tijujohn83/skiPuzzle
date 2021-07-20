
using System.Diagnostics;

namespace Problem1
{
    [DebuggerDisplay("{X}, {Y}, {Z}, {LengthFromPeak}, {IsPeak.hasValue ? IsPeak.Value.ToString() : string.Empty}")]
    public struct LandScapeCell
    {
        public int X;
        public int Y;
        public int Z;
        public int LengthFromPeak;
        public bool? IsPeak;

        public LandScapeCell(int x, int y, int z, int lengthFromPeak)
        {
            X = x;
            Y = y;
            Z = z;
            LengthFromPeak = lengthFromPeak;
            IsPeak = null;
        }
    }

}
