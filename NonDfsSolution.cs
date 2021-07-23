using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Problem1
{
    public class NonDfsSolution : ISolution
    {
        private static readonly object LockObj = new object();

        public IEnumerable<Solution> Solve()
        {
            var solutions = new List<Solution>();
            Parallel.For(0, LandScapeMatrix.MatrixLength, x =>
            {
                for (var y = 0; y < LandScapeMatrix.MatrixLength; y++)
                {
                    SolveForPeaks(x, y, solutions);
                }
            });

            return solutions;
        }

        private static void SolveForPeaks(int x, int y, List<Solution> solutions)
        {
            var currentCell = LandScapeMatrix.Cells[x, y];
            if (currentCell.IsPeak.HasValue) return;

            var current = currentCell.Z;

            int left;
            if (y - 1 < 0)
                left = current;
            else
            {
                left = LandScapeMatrix.Cells[x, y - 1].Z;
                if (current > left) LandScapeMatrix.Cells[x, y - 1].IsPeak = false;
            }

            int right;
            if (y + 1 >= LandScapeMatrix.MatrixLength)
                right = current;
            else
            {
                right = LandScapeMatrix.Cells[x, y + 1].Z;
                if (current > right) LandScapeMatrix.Cells[x, y + 1].IsPeak = false;
            }

            int top;
            if (x - 1 < 0)
                top = current;
            else
            {
                top = LandScapeMatrix.Cells[x - 1, y].Z;
                if (current > top) LandScapeMatrix.Cells[x - 1, y].IsPeak = false;
            }

            int bottom;
            if (x + 1 >= LandScapeMatrix.MatrixLength)
                bottom = current;
            else
            {
                bottom = LandScapeMatrix.Cells[x + 1, y].Z;
                if (current > bottom) LandScapeMatrix.Cells[x + 1, y].IsPeak = false;
            }

            LandScapeMatrix.Cells[x, y].IsPeak = current >= left
                                              && current >= right
                                              && current >= top
                                              && current >= bottom;


            if (LandScapeMatrix.Cells[x, y].IsPeak.Value)
            {
                var solutionsForThisPeak = SolveForPeak(x, y).ToList();

                // take any because all solutions in the top set will have same number of hops and depth
                var firstSolutionThisPeak = solutionsForThisPeak.FirstOrDefault();
                var currentFirstSolution = solutions.FirstOrDefault();

                lock (LockObj)
                {
                    if (firstSolutionThisPeak != null)
                    {
                        //if no current solutions, then add the latest
                        if (currentFirstSolution == null)
                        {
                            solutions.AddRange(solutionsForThisPeak);
                        }
                        //this peak solutions have more hops
                        else if (firstSolutionThisPeak.Hops > currentFirstSolution.Hops)
                        {
                            solutions.Clear();
                            solutions.AddRange(solutionsForThisPeak);
                        }
                        //this peak solutions have same hops but more depth
                        else if (firstSolutionThisPeak.Hops == currentFirstSolution.Hops && firstSolutionThisPeak.Depth > currentFirstSolution.Depth)
                        {
                            solutions.Clear();
                            solutions.AddRange(solutionsForThisPeak);
                        }
                        //this peak solutions have same hops and same depth
                        else if (firstSolutionThisPeak.Hops == currentFirstSolution.Hops && firstSolutionThisPeak.Depth == currentFirstSolution.Depth)
                        {
                            solutions.AddRange(solutionsForThisPeak);
                        }
                    }
                }
            }


        }

        private static IEnumerable<Solution> SolveForPeak(int x, int y)
        {
            var allSolutions = ReturnAllSolutions(x, y);

            if (allSolutions.Any())
            {

                var maxHops = allSolutions.OrderByDescending(s => s.Hops).FirstOrDefault().Hops;
                var maxDepth = allSolutions.Where(s => s.Hops == maxHops).OrderByDescending(s => s.Depth)
                    .FirstOrDefault().Depth;

                return ReturnAllSolutions(x, y)
                    .Where(s => s.Hops == maxHops && s.Depth == maxDepth);
            }

            return Enumerable.Empty<Solution>();
        }

        private static IEnumerable<Solution> ReturnAllSolutions(int x, int y)
        {
            var landScape = LandScapeMatrix.Cells;
            var currentCell = LandScapeMatrix.Cells[x, y];
            var solutions = new List<Solution>();
            var isLeafCell = true;

            //left
            if (y > 0 && landScape[x, y - 1].Z < landScape[x, y].Z)
            {
                solutions.AddRange(ReturnAllSolutions(x, y - 1).Select(leaf =>
                {
                    leaf.Path.Insert(0, currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            //right
            if (y < LandScapeMatrix.MatrixLength - 1 && landScape[x, y + 1].Z < landScape[x, y].Z)
            {
                solutions.AddRange(ReturnAllSolutions(x, y + 1).Select(leaf =>
                {
                    leaf.Path.Insert(0, currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            //top
            if (x > 0 && landScape[x - 1, y].Z < landScape[x, y].Z)
            {
                solutions.AddRange(ReturnAllSolutions(x - 1, y).Select(leaf =>
                {
                    leaf.Path.Insert(0, currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            //bottom
            if (x < LandScapeMatrix.MatrixLength - 1 && landScape[x + 1, y].Z < landScape[x, y].Z)
            {
                solutions.AddRange(ReturnAllSolutions(x + 1, y).Select(leaf =>
                {
                    leaf.Path.Insert(0, currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            if (isLeafCell)
                solutions.Add(new Solution { Path = new List<LandScapeCell> { currentCell } });


            return solutions;
        }
    }
}
