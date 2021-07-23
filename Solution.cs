using System.Collections.Generic;
using System.Linq;

namespace SkiPuzzle
{
    public class Solution
    {
        public int Depth => Hops == 0 ? 0 : Path.First().Z - Path.Last().Z;
        public int Hops => Path.Count;

        public List<LandScapeCell> Path = new List<LandScapeCell>();
    }
}
