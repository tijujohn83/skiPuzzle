using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Problem1
{
    public class NonDfsSolution : ISolution
    {
        private static readonly object LockObj = new object();

        public Solution Solve()
        {
            var solution = new Solution { Depth = 0, Length = 0, Path = new List<LandScapeCell>() };

            Parallel.For(0, LandScapeMatrix.SquareMapSide, x =>
            {
                Parallel.For(0, LandScapeMatrix.SquareMapSide, y =>
                {
                    SolveForPeaks(x, y, solution);
                });
            });
            solution.Path.Reverse();
            return solution;
        }

        private static void SolveForPeaks(int x, int y, Solution solution)
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
            if (y + 1 >= LandScapeMatrix.SquareMapSide)
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
            if (x + 1 >= LandScapeMatrix.SquareMapSide)
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
                var peak = SolveForPeak(x, y);

                lock (LockObj)
                {
                    //longest then steepest
                    if (peak.Length > solution.Length)
                    {
                        solution.Length = peak.Length;
                        solution.Depth = peak.Depth;
                        solution.Path = peak.Path;
                    } else if (peak.Length == solution.Length)
                        if (peak.Depth > solution.Depth)
                        {
                            solution.Length = peak.Length;
                            solution.Depth = peak.Depth;
                            solution.Path = peak.Path;
                        }
                }
            }


        }

        private static Solution SolveForPeak(int x, int y)
        {
            var landScape = LandScapeMatrix.Cells;
            var leafCells = ReturnAllLeaves(x, y);
            var solutionCell = leafCells.OrderByDescending(cell => cell.LongestPath.Count).ThenBy(cell => cell.Z).FirstOrDefault();
            return new Solution { Depth = landScape[x, y].Z - solutionCell.Z, Length = solutionCell.LongestPath.Count, Path = solutionCell.LongestPath.ToList() };
        }

        private static IEnumerable<LandScapeCell> ReturnAllLeaves(int x, int y)
        {
            var landScape = LandScapeMatrix.Cells;
            var currentCell = LandScapeMatrix.Cells[x, y];
            var leafCells = new List<LandScapeCell>();
            var isLeafCell = true;

            //left
            if (y > 0 && landScape[x, y - 1].Z < landScape[x, y].Z)
            {
                leafCells.AddRange(ReturnAllLeaves(x, y - 1).Select(leaf =>
                {
                    leaf.LongestPath.Add(currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            //right
            if (y < LandScapeMatrix.SquareMapSide - 1 && landScape[x, y + 1].Z < landScape[x, y].Z)
            {
                leafCells.AddRange(ReturnAllLeaves(x, y + 1).Select(leaf =>
                {
                    leaf.LongestPath.Add(currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            //top
            if (x > 0 && landScape[x - 1, y].Z < landScape[x, y].Z)
            {
                leafCells.AddRange(ReturnAllLeaves(x - 1, y).Select(leaf =>
                {
                    leaf.LongestPath.Add(currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            //bottom
            if (x < LandScapeMatrix.SquareMapSide - 1 && landScape[x + 1, y].Z < landScape[x, y].Z)
            {
                leafCells.AddRange(ReturnAllLeaves(x + 1, y).Select(leaf =>
                {
                    leaf.LongestPath.Add(currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            if (isLeafCell)
                leafCells.Add(new LandScapeCell(x, y, landScape[x, y].Z) { LongestPath = new List<LandScapeCell> { currentCell } });


            return leafCells;
        }
    }
}
