using System.Diagnostics;

namespace SkiPuzzle
{
    [DebuggerDisplay("{Z}({X},{Y}), {IsPeak.hasValue ? IsPeak.Value.ToString() : null}")]
    public class LandScapeCell
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;
        public bool? IsPeak;

        public LandScapeCell(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

    }

}
