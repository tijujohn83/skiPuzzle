using System;
using System.Collections.Generic;

//Answer:15-1422
namespace Problem1
{
    class Program
    {
        public static Dictionary<string, IEnumerable<LandScapeCell>> Lookup = new Dictionary<string, IEnumerable<LandScapeCell>>();
        public static bool?[,] Peaks = new bool?[LandScapeMatrix.SquareMapSide, LandScapeMatrix.SquareMapSide];

        static void Main(string[] args)
        {
            var solution = new TreeLengthDepth { Depth = 0, Length = 0 };

            for (var i = 0; i < LandScapeMatrix.SquareMapSide; i++)
            for (var j = 0; j < LandScapeMatrix.SquareMapSide; j++)
                SolveForPeaks(i, j, ref solution);

            Console.WriteLine(solution.Length + "-" + solution.Depth);
            Console.ReadKey();

        }

        private static void SolveForPeaks(int x, int y, ref TreeLengthDepth solution)
        {
            if (Peaks[x, y].HasValue) return;

            var current = LandScapeMatrix.ReducedLandScape[x, y];

            int left;
            if (x - 1 < 0)
                left = current;
            else
            {
                left = LandScapeMatrix.ReducedLandScape[x - 1, y];
                if (current > left) Peaks[x - 1, y] = false;
            }

            int right;
            if (x + 1 >= LandScapeMatrix.SquareMapSide)
                right = current;
            else
            {
                right = LandScapeMatrix.ReducedLandScape[x + 1, y];
                if (current > right) Peaks[x + 1, y] = false;
            }

            int top;
            if (y - 1 < 0)
                top = current;
            else
            {
                top = LandScapeMatrix.ReducedLandScape[x, y - 1];
                if (current > top) Peaks[x, y - 1] = false;
            }

            int bottom;
            if (y + 1 >= LandScapeMatrix.SquareMapSide)
                bottom = current;
            else
            {
                bottom = LandScapeMatrix.ReducedLandScape[x, y + 1];
                if (current > bottom) Peaks[x, y + 1] = false;
            }

            Peaks[x, y] = current >= left
                          && current >= right
                          && current >= top
                          && current >= bottom;


            if (Peaks[x, y].Value)
            {
                var peak = SolveForPeak(x, y);

                //longest then steepest
                if (peak.Length > solution.Length)
                    solution = peak;
                else if (peak.Length == solution.Length)
                    if (peak.Depth > solution.Depth)
                        solution = peak;
            }

        }

        private static TreeLengthDepth SolveForPeak(int x, int y)
        {
            var landScape = LandScapeMatrix.ReducedLandScape;
            var solution = new TreeLengthDepth { Depth = 0, Length = 0 };

            var leafCells = ReturnAllLeaves(x, y, 1);

            //longest then steepest
            foreach (var leafCell in leafCells)
            {
                if (leafCell.LengthFromRoot > solution.Length)
                {
                    solution.Length = leafCell.LengthFromRoot;
                    solution.Depth = landScape[x, y] - leafCell.Z;
                } else if (leafCell.LengthFromRoot == solution.Length)
                    if (landScape[x, y] - leafCell.Z > solution.Depth)
                    {
                        solution.Depth = landScape[x, y] - leafCell.Z;
                    }
            }

            return solution;
        }
        
        private static IEnumerable<LandScapeCell> ReturnAllLeaves(int x, int y, int hops)
        {
            if (Lookup.TryGetValue(x.ToString() + y.ToString() + hops.ToString(), out var cache))
            {
                return cache;
            }

            var landScape = LandScapeMatrix.ReducedLandScape;

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
            if (y > 0 && (landScape[x, y - 1] < landScape[x, y]))
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


            Lookup[x.ToString() + y.ToString() + hops.ToString()] = leafCells;
            return leafCells;
        }
        
    }
}

