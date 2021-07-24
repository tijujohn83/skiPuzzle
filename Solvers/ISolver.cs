using System.Collections.Generic;
using SkiPuzzle.Model;

namespace skiPuzzle.Solvers
{
    public interface ISolver
    {
        List<Solution> Solve(LandScapeMatrix landScapeMatrix);
    }
}