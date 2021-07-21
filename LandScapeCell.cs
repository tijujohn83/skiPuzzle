
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;

namespace Problem1
{
    [DebuggerDisplay("{X}, {Y}, {Z}, {LengthFromPeak}, {IsPeak.hasValue ? IsPeak.Value.ToString() : null}")]
    public class LandScapeCell
    {
        public int X;
        public int Y;
        public int Z;
        public int LengthFromPeak;
        public bool? IsPeak;
        public List<LandScapeCell> Path;

        public LandScapeCell()
        {
            
        }

        public LandScapeCell(int x, int y, int z, int lengthFromPeak)
        {
            X = x;
            Y = y;
            Z = z;
            LengthFromPeak = lengthFromPeak;
            //IsPeak = null;
        }

        public void CopyFrom(LandScapeCell source)
        {
            X = source.X;
            Y = source.Y;
            Z = source.Z;
            LengthFromPeak = source.LengthFromPeak;
            IsPeak = source.IsPeak;
        }
    }

}
