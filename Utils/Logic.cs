using System.Collections.Generic;
using System.Linq;
using SkiPuzzle.Model;

namespace SkiPuzzle.Utils
{
    public static class Logic
    {
        public static void Merge(this List<Solution> currentSolutions, List<Solution> newSolutions)
        {
            // take any because all currentSolutions in the top set will have same number of hops and depth
            var firstNewSolution = newSolutions.FirstOrDefault();
            var firstCurrentSolution = currentSolutions.FirstOrDefault();

            if (firstNewSolution != null)
            {
                //if no current currentSolutions, then add the latest
                if (firstCurrentSolution == null)
                {
                    currentSolutions.AddRange(newSolutions);
                }
                //this peak currentSolutions have more hops
                else if (firstNewSolution.Hops > firstCurrentSolution.Hops)
                {
                    currentSolutions.Clear();
                    currentSolutions.AddRange(newSolutions);
                }
                //this peak currentSolutions have same hops but more depth
                else if (firstNewSolution.Hops == firstCurrentSolution.Hops && firstNewSolution.Depth > firstCurrentSolution.Depth)
                {
                    currentSolutions.Clear();
                    currentSolutions.AddRange(newSolutions);
                }
                //this peak currentSolutions have same hops and same depth
                else if (firstNewSolution.Hops == firstCurrentSolution.Hops && firstNewSolution.Depth == firstCurrentSolution.Depth)
                {
                    currentSolutions.AddRange(newSolutions);
                }

            }

        }

        public static List<Solution> FindSolutions(this List<Solution> possibleSolutions)
        {
            if (!possibleSolutions.Any()) return possibleSolutions;

            var maxHops = possibleSolutions.OrderByDescending(s => s.Hops).FirstOrDefault()?.Hops ?? 0;
            var maxDepth = possibleSolutions.Where(s => s.Hops == maxHops).OrderByDescending(s => s.Depth)
                .FirstOrDefault()?.Depth ?? 0;

            return possibleSolutions.Where(s => s.Hops == maxHops && s.Depth == maxDepth).ToList();
        }
    }
}
