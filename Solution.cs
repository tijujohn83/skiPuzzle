using System.Collections.Generic;
using System.Linq;

namespace Problem1
{
    public class Solution
    {
        public int Depth => Path.First().Z - Path.Last().Z;
        public int Length => Path.Count;

        public List<LandScapeCell> Path;
    }
}
