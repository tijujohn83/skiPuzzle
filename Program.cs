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
        public static bool?[,] Peaks = new bool?[LandScapeMatrix.SquareMapSide, LandScapeMatrix.SquareMapSide];

        static void Main(string[] args)
        {
            var solution = new TreeLengthDepth { Depth = 0, Length = 0 };

            Parallel.For(0, LandScapeMatrix.SquareMapSide, i =>
            {
                Parallel.For(0, LandScapeMatrix.SquareMapSide, j =>
                {
                    var x = i;
                    var y = j;
                    SolveForPeaks(x, y, ref solution);
                });
            });

            Console.WriteLine(solution.Length + "-" + solution.Depth);
            Console.ReadKey();

        }

        private static void SolveForPeaks(int x, int y, ref TreeLengthDepth solution)
        {
            if (Peaks[x, y].HasValue) return;

            var current = LandScapeMatrix.LandScape[x, y];

            int left;
            if (x - 1 < 0)
                left = current;
            else
            {
                left = LandScapeMatrix.LandScape[x - 1, y];
                if (current > left) Peaks[x - 1, y] = false;
            }

            int right;
            if (x + 1 >= LandScapeMatrix.SquareMapSide)
                right = current;
            else
            {
                right = LandScapeMatrix.LandScape[x + 1, y];
                if (current > right) Peaks[x + 1, y] = false;
            }

            int top;
            if (y - 1 < 0)
                top = current;
            else
            {
                top = LandScapeMatrix.LandScape[x, y - 1];
                if (current > top) Peaks[x, y - 1] = false;
            }

            int bottom;
            if (y + 1 >= LandScapeMatrix.SquareMapSide)
                bottom = current;
            else
            {
                bottom = LandScapeMatrix.LandScape[x, y + 1];
                if (current > bottom) Peaks[x, y + 1] = false;
            }

            Peaks[x, y] = current >= left
                          && current >= right
                          && current >= top
                          && current >= bottom;


            if (Peaks[x, y].Value)
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

        private static TreeLengthDepth SolveForPeak(int x, int y)
        {
            var landScape = LandScapeMatrix.LandScape;
            var leafCells = ReturnAllLeaves(x, y, 1);
            var solutionCell = leafCells.OrderByDescending(cell => cell.LengthFromRoot).ThenBy(cell => cell.Z).FirstOrDefault();
            return new TreeLengthDepth { Depth = landScape[x, y] - solutionCell.Z, Length = solutionCell.LengthFromRoot };
        }

        private static IEnumerable<LandScapeCell> ReturnAllLeaves(int x, int y, int hops)
        {
            if (Lookup.TryGetValue(LookupKey(x, y, hops), out var cache))
            {
                return cache;
            }

            var landScape = LandScapeMatrix.LandScape;

            var leafCells = new List<LandScapeCell>();
            var isLeafCell = true;

            //left
            if (x > 0 && landScape[x - 1, y] < landScape[x, y])
            {
                leafCells.AddRange(ReturnAllLeaves(x - 1, y, hops + 1));
                isLeafCell = false;
            }

            //right
            if (x < LandScapeMatrix.SquareMapSide - 1 && landScape[x + 1, y] < landScape[x, y])
            {
                leafCells.AddRange(ReturnAllLeaves(x + 1, y, hops + 1));
                isLeafCell = false;
            }

            //top
            if (y > 0 && landScape[x, y - 1] < landScape[x, y])
            {
                leafCells.AddRange(ReturnAllLeaves(x, y - 1, hops + 1));
                isLeafCell = false;
            }

            //bottom
            if (y < LandScapeMatrix.SquareMapSide - 1 && landScape[x, y + 1] < landScape[x, y])
            {
                leafCells.AddRange(ReturnAllLeaves(x, y + 1, hops + 1));
                isLeafCell = false;
            }

            if (isLeafCell)
                leafCells.Add(new LandScapeCell(x, y, landScape[x, y], hops));


            Lookup.TryAdd(LookupKey(x, y, hops), leafCells);
            return leafCells;
        }

        public static string LookupKey(int x, int y, int z)
        {
            return x + "-" + y + "-" + z;
        }
    }
}

