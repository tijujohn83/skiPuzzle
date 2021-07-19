using System;
using System.Collections.Generic;

namespace Problem1
{
    class Program
    {       
        static void Main(string[] args)
        {
            var landScape = LandScapeMatrix.ReducedLandScape;
            var remainingHeight = LandScapeMatrix.MaxHeight - LandScapeMatrix.MinHeight;
            var solution = new TreeLengthDepth { Depth = 0, Length = 0 };
            
            //ToDo: Optimization: instead of going through all cells, save the list of peaks in the initial passes and just iterate those.
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
            var area = LandScapeMatrix.ReducedLandScape;
            var solution = new TreeLengthDepth { Depth = 0, Length = 0 };

            var leafCells = ReturnLeaves(x, y, 1);

            //longest then steepest
            foreach (var leafCell in leafCells)
            {
                if (leafCell.LengthFromRoot > solution.Length)
                {
                    solution.Length = leafCell.LengthFromRoot;
                    solution.Depth = area[x, y] - leafCell.Z;
                }
                else if (leafCell.LengthFromRoot == solution.Length)
                    if (area[x, y] - leafCell.Z > solution.Depth)
                    {
                        solution.Depth = area[x, y] - leafCell.Z;
                    }
            }

            return solution;
        }

        private static IEnumerable<LandScapeCell> ReturnLeaves(int x, int y, int length)
        {
            var landScape = LandScapeMatrix.ReducedLandScape;

            var leafCells = new List<LandScapeCell>();
            var addParentCell = true;

            //left
            if (x > 0 && landScape[x - 1, y] < landScape[x, y])
            {
                leafCells.AddRange(ReturnLeaves(x - 1, y, length + 1));
                addParentCell = false;
            }

            //right
            if (x < LandScapeMatrix.SquareMapSide - 1 && landScape[x + 1, y] < landScape[x, y])
            {
                leafCells.AddRange(ReturnLeaves(x + 1, y, length + 1));
                addParentCell = false;
            }

            //top
            if (y > 0 && (landScape[x, y - 1] < landScape[x, y]))
            {
                leafCells.AddRange(ReturnLeaves(x, y - 1, length + 1));
                addParentCell = false;
            }

            //bottom
            if (y < LandScapeMatrix.SquareMapSide - 1 && landScape[x, y + 1] < landScape[x, y])
            {
                leafCells.AddRange(ReturnLeaves(x, y + 1, length + 1));
                addParentCell = false;
            }

            if (addParentCell)
                leafCells.Add(new LandScapeCell(x, y, landScape[x, y], length));

            return leafCells;
        }
    }
}

