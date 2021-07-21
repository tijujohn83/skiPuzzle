
using System.Collections.Generic;
using System.Diagnostics;

namespace Problem1
{
    [DebuggerDisplay("{X}, {Y}, {Z}, {CellsTraversed}, {IsPeak.hasValue ? IsPeak.Value.ToString() : null}")]
    public class LandScapeCell
    {
        public int X;
        public int Y;
        public int Z;
        public int CellsTraversed;
        public bool? IsPeak;
        public List<LandScapeCell> Path;

        public LandScapeCell()
        {
            
        }

        public LandScapeCell(int x, int y, int z, int cellsTraversed)
        {
            X = x;
            Y = y;
            Z = z;
            CellsTraversed = cellsTraversed;
            //IsPeak = null;
        }

        public void UpdateSolution(LandScapeCell newSolution)
        {
            X = newSolution.X;
            Y = newSolution.Y;
            Z = newSolution.Z;
            CellsTraversed = newSolution.CellsTraversed;
            IsPeak = newSolution.IsPeak;
            Path = Path ?? new List<LandScapeCell>();
            Path.AddRange(newSolution.Path);
        }
    }

}
