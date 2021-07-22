using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Problem1
{
    [DebuggerDisplay("{Z}({X},{Y}), [{PathString}], {IsPeak.hasValue ? IsPeak.Value.ToString() : null}")]
    public class LandScapeCell
    {
        public int X;
        public int Y;
        public int Z;
        //public int LongestCellsTraversed;
        public bool? IsPeak;
        public List<LandScapeCell> LongestPath;

        public string PathString
        {
            get
            {
                return LongestPath != null ? string.Join("-> ", LongestPath.Select(p => $"({p.X},{p.Y})")) : "";
            }
        }

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
