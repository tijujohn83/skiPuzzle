using System;
using System.Collections.Generic;

//Answer:15-1422
namespace Problem1
{
    class Program
    {
        public static Dictionary<string, IEnumerable<LandScapeCell>> Lookup = new Dictionary<string, IEnumerable<LandScapeCell>>();

        static void Main(string[] args)
        {
            var landScape = LandScapeMatrix.ReducedLandScape;
            var remainingHeight = LandScapeMatrix.MaxHeight - LandScapeMatrix.MinHeight;
            
            var solution = new TreeLengthDepth { Depth = 0, Length = 0 };
            
            //ToDo: Optimization: instead of going through all cells, save the list of peaks in the initial passes and just iterate those.
            //solve from all peaks and pick the best
            while (remainingHeight > 0)
            {
                for (var i = 0; i < LandScapeMatrix.SquareMapSide; i++)
                {
                    for (var j = 0; j < LandScapeMatrix.SquareMapSide; j++)
                    {
                        if (landScape[i, j] == remainingHeight) 
                        {
                            var peak = SolveForPeak(i, j);

                            //longest then steepest
                            if (peak.Length > solution.Length)
                                solution = peak;
                            else if (peak.Length == solution.Length)
                                if (peak.Depth > solution.Depth)
                                    solution = peak;
                        }
                    }
                }

                remainingHeight--;
            }

            Console.WriteLine(solution.Length + "-" + solution.Depth);
            Console.ReadKey();

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
                }
                else if (leafCell.LengthFromRoot == solution.Length)
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

