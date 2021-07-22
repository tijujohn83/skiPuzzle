using System;
using System.Collections.Generic;
using System.Linq;

namespace Problem1
{
    public class DfsSolution : ISolution
    {
        private static readonly object LockObj = new object();

        public Solution Solve()
        {
            var solution = new Solution { Depth = 0, Length = 0, LongestPath = new List<LandScapeCell>() };

            for (var x = 0; x < LandScapeMatrix.SquareMapSide; x++)
            {
                for (var y = 0; y < LandScapeMatrix.SquareMapSide; y++)
                {
                    SolveForPeaksDfs(x, y, solution);
                }
            }
            solution.LongestPath.Reverse();
            return solution;
        }

        private static void SolveForPeaksDfs(int x, int y, Solution solution)
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
                var peak = SolveForPeakDfs(x, y);

                lock (LockObj)
                {
                    //longest then steepest
                    if (peak.Length > solution.Length)
                    {
                        solution.Length = peak.Length;
                        solution.Depth = peak.Depth;
                        solution.LongestPath = peak.LongestPath;
                    } else if (peak.Length == solution.Length)
                        if (peak.Depth > solution.Depth)
                        {
                            solution.Length = peak.Length;
                            solution.Depth = peak.Depth;
                            solution.LongestPath = peak.LongestPath;
                        }
                }
            }

        }

        private static Solution SolveForPeakDfs(int x, int y)
        {
            return Dfs(x, y, new List<LandScapeCell>());
        }

        private static Solution Dfs(int x, int y, IEnumerable<LandScapeCell> path)
        {
            var landScape = LandScapeMatrix.Cells;
            var currentCell = landScape[x, y];
            var longestPath = new List<LandScapeCell>(path) { currentCell };
            var solution = new Solution { LongestPath = longestPath };
            solution.LongestPath.Add(currentCell);


            Solution PickBetter(Solution sol1, Solution sol2)
            {
                if (sol1.LongestPath.Count > sol2.LongestPath.Count)
                {
                    return sol1;
                }

                if (sol1.LongestPath.Count == sol2.LongestPath.Count && sol1.Depth < sol2.Depth)
                {
                    return sol1;
                }

                return sol2;
            }

            //left
            if (y > 0 && landScape[x, y - 1].Z < landScape[x, y].Z)
            {
                var leftSol = Dfs(x, y - 1, solution.LongestPath);
                solution = PickBetter(leftSol, solution);
            }

            //right
            if (y < LandScapeMatrix.SquareMapSide - 1 && landScape[x, y + 1].Z < landScape[x, y].Z)
            {
                var rightSol = Dfs(x, y + 1, solution.LongestPath);
                solution = PickBetter(rightSol, solution);
            }

            //top
            if (x > 0 && landScape[x - 1, y].Z < landScape[x, y].Z)
            {
                var topSol = Dfs(x - 1, y, solution.LongestPath);
                solution = PickBetter(topSol, solution);
            }

            //bottom
            if (x < LandScapeMatrix.SquareMapSide - 1 && landScape[x + 1, y].Z < landScape[x, y].Z)
            {
                var bottomSol = Dfs(x + 1, y, solution.LongestPath);
                solution = PickBetter(bottomSol, solution);
            }

            return solution;
        }
    }
}
