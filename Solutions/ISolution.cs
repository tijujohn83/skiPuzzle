using System.Collections.Generic;
using SkiPuzzle.Model;

namespace skiPuzzle.Solutions
{
    public interface ISolution
    {
        List<Solution> Solve(LandScapeMatrix landScapeMatrix);
    }
}