using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Answer:15-1422
namespace Problem1
{
    class Program
    {
        private static readonly object LockObj = new object();
        public static ConcurrentDictionary<string, IEnumerable<LandScapeCell>> Lookup = new ConcurrentDictionary<string, IEnumerable<LandScapeCell>>();

        static void Main(string[] args)
        {
            var solution = new TreeLengthDepth { Depth = 0, Length = 0 };

            solution = NonDfs(solution);
            //solution = Dfs(solution);

            Console.WriteLine(solution.Length + "-" + solution.Depth);
            Console.ReadKey();

        }

        private static TreeLengthDepth Dfs(TreeLengthDepth solution)
        {
            for (var i = 0; i < LandScapeMatrix.SquareMapSide; i++)
            {
                for (var j = 0; i < LandScapeMatrix.SquareMapSide; i++)
                {
                    var x = i;
                    var y = j;
                    SolveForPeaksDfs(x, y, ref solution);
                }
            }

            return solution;
        }

        private static TreeLengthDepth NonDfs(TreeLengthDepth solution)
        {
            Parallel.For(0, LandScapeMatrix.SquareMapSide, i =>
            {
                Parallel.For(0, LandScapeMatrix.SquareMapSide, j =>
                {
                    var x = i;
                    var y = j;
                    SolveForPeaks(x, y, ref solution);
                });
            });
            return solution;
        }

        private static void SolveForPeaks(int x, int y, ref TreeLengthDepth solution)
        {
            if (LandScapeMatrix.Cells[x, y].IsPeak.HasValue) return;

            var current = LandScapeMatrix.Cells[x, y].Z;

            int left;
            if (x - 1 < 0)
                left = current;
            else
            {
                left = LandScapeMatrix.Cells[x - 1, y].Z;
                if (current > left) LandScapeMatrix.Cells[x - 1, y].IsPeak = false;
            }

            int right;
            if (x + 1 >= LandScapeMatrix.SquareMapSide)
                right = current;
            else
            {
                right = LandScapeMatrix.Cells[x + 1, y].Z;
                if (current > right) LandScapeMatrix.Cells[x + 1, y].IsPeak = false;
            }

            int top;
            if (y - 1 < 0)
                top = current;
            else
            {
                top = LandScapeMatrix.Cells[x, y - 1].Z;
                if (current > top) LandScapeMatrix.Cells[x, y - 1].IsPeak = false;
            }

            int bottom;
            if (y + 1 >= LandScapeMatrix.SquareMapSide)
                bottom = current;
            else
            {
                bottom = LandScapeMatrix.Cells[x, y + 1].Z;
                if (current > bottom) LandScapeMatrix.Cells[x, y + 1].IsPeak = false;
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
                    } else if (peak.Length == solution.Length)
                        if (peak.Depth > solution.Depth)
                        {
                            solution.Length = peak.Length;
                            solution.Depth = peak.Depth;
                        }
                }
            }

        }

        private static void SolveForPeaksDfs(int x, int y, ref TreeLengthDepth solution)
        {
            if (LandScapeMatrix.Cells[x, y].IsPeak.HasValue) return;

            var current = LandScapeMatrix.Cells[x, y].Z;

            int left;
            if (x - 1 < 0)
                left = current;
            else
            {
                left = LandScapeMatrix.Cells[x - 1, y].Z;
                if (current > left) LandScapeMatrix.Cells[x - 1, y].IsPeak = false;
            }

            int right;
            if (x + 1 >= LandScapeMatrix.SquareMapSide)
                right = current;
            else
            {
                right = LandScapeMatrix.Cells[x + 1, y].Z;
                if (current > right) LandScapeMatrix.Cells[x + 1, y].IsPeak = false;
            }

            int top;
            if (y - 1 < 0)
                top = current;
            else
            {
                top = LandScapeMatrix.Cells[x, y - 1].Z;
                if (current > top) LandScapeMatrix.Cells[x, y - 1].IsPeak = false;
            }

            int bottom;
            if (y + 1 >= LandScapeMatrix.SquareMapSide)
                bottom = current;
            else
            {
                bottom = LandScapeMatrix.Cells[x, y + 1].Z;
                if (current > bottom) LandScapeMatrix.Cells[x, y + 1].IsPeak = false;
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
                    } else if (peak.Length == solution.Length)
                        if (peak.Depth > solution.Depth)
                        {
                            solution.Length = peak.Length;
                            solution.Depth = peak.Depth;
                        }
                }
            }

        }

        private static TreeLengthDepth SolveForPeak(int x, int y)
        {
            var landScape = LandScapeMatrix.Cells;
            var leafCells = ReturnAllLeaves(x, y, 1);
            var solutionCell = leafCells.OrderByDescending(cell => cell.LengthFromPeak).ThenBy(cell => cell.Z).FirstOrDefault();
            return new TreeLengthDepth { Depth = landScape[x, y].Z - solutionCell.Z, Length = solutionCell.LengthFromPeak };
        }

        private static TreeLengthDepth SolveForPeakDfs(int x, int y)
        {
            var landScape = LandScapeMatrix.Cells;
            var solutionCell = Dfs(x, y, 1);
            return new TreeLengthDepth { Depth = landScape[x, y].Z - solutionCell.Z, Length = solutionCell.LengthFromPeak };
        }

        private static LandScapeCell Dfs(int x, int y, int hops)
        {
            void UpdateSolution(LandScapeCell newSol, ref LandScapeCell currSol)
            {
                if (newSol.LengthFromPeak > currSol.LengthFromPeak)
                {
                    currSol = newSol;
                } else if (newSol.LengthFromPeak == currSol.LengthFromPeak && newSol.Z < currSol.Z)
                {
                    currSol = newSol;
                }
            }

            var landScape = LandScapeMatrix.Cells;
            var solutionCell = landScape[x, y];

            //left
            if (x > 0 && landScape[x - 1, y].Z < landScape[x, y].Z)
            {
                UpdateSolution(Dfs(x - 1, y, hops + 1), ref solutionCell);
            }

            //right
            if (x < LandScapeMatrix.SquareMapSide - 1 && landScape[x + 1, y].Z < landScape[x, y].Z)
            {
                UpdateSolution(Dfs(x + 1, y, hops + 1), ref solutionCell);
            }

            //top
            if (y > 0 && landScape[x, y - 1].Z < landScape[x, y].Z)
            {
                UpdateSolution(Dfs(x, y - 1, hops + 1), ref solutionCell);
            }

            //bottom
            if (y < LandScapeMatrix.SquareMapSide - 1 && landScape[x, y + 1].Z < landScape[x, y].Z)
            {
                UpdateSolution(Dfs(x, y + 1, hops + 1), ref solutionCell);
            }

            solutionCell.LengthFromPeak = hops;

            return solutionCell;
        }

        private static IEnumerable<LandScapeCell> ReturnAllLeaves(int x, int y, int hops)
        {
            if (Lookup.TryGetValue(LookupKey(x, y, hops), out var cache))
            {
                return cache;
            }

            var landScape = LandScapeMatrix.Cells;

            var leafCells = new List<LandScapeCell>();
            var isLeafCell = true;

            //left
            if (x > 0 && landScape[x - 1, y].Z < landScape[x, y].Z)
            {
                leafCells.AddRange(ReturnAllLeaves(x - 1, y, hops + 1));
                isLeafCell = false;
            }

            //right
            if (x < LandScapeMatrix.SquareMapSide - 1 && landScape[x + 1, y].Z < landScape[x, y].Z)
            {
                leafCells.AddRange(ReturnAllLeaves(x + 1, y, hops + 1));
                isLeafCell = false;
            }

            //top
            if (y > 0 && landScape[x, y - 1].Z < landScape[x, y].Z)
            {
                leafCells.AddRange(ReturnAllLeaves(x, y - 1, hops + 1));
                isLeafCell = false;
            }

            //bottom
            if (y < LandScapeMatrix.SquareMapSide - 1 && landScape[x, y + 1].Z < landScape[x, y].Z)
            {
                leafCells.AddRange(ReturnAllLeaves(x, y + 1, hops + 1));
                isLeafCell = false;
            }

            if (isLeafCell)
                leafCells.Add(new LandScapeCell(x, y, landScape[x, y].Z, hops));


            Lookup.TryAdd(LookupKey(x, y, hops), leafCells);
            return leafCells;
        }

        public static string LookupKey(int x, int y, int z)
        {
            return x + "-" + y + "-" + z;
        }
    }
}

