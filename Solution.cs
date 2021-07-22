using System.Collections.Generic;
using System.Linq;

namespace Problem1
{
    public class Solution
    {
        public int Depth => LongestPath.First().Z - LongestPath.Last().Z;
        public int Length => LongestPath.Count;

        public List<LandScapeCell> LongestPath;
    }
}
