using System;
using System.Collections.Generic;

namespace Problem1
{
    class Program
    {       
        static void Main(string[] args)
        {
            var landScape = LandScapeMatrix.ReducedLandScape;
            var steepestPossibleJump = LandScapeMatrix.MaxHeight - LandScapeMatrix.MinHeight;
            var solution = new TreeLengthDepth { Depth = 0, Length = 0 };
            //later: instead of going through all lengths, save the list of peaks in the initial passes.
            while (steepestPossibleJump > 0)
            {
                for (var i = 0; i < LandScapeMatrix.SquareMapSide; i++)
                {
                    for (var j = 0; j < LandScapeMatrix.SquareMapSide; j++)
                    {
                        if (landScape[i, j] == steepestPossibleJump) //run for only all available peaks
                        {
                            var peak = SolveForPeak(i, j);

                            if (peak.Length > solution.Length)
                                solution = peak;
                            else if (peak.Length == solution.Length)
                                if (peak.Depth > solution.Depth)
                                    solution = peak;
                        }
                    }
                }

                steepestPossibleJump--;
            }

            Console.WriteLine(solution.Length + "-" + solution.Depth);
            Console.ReadKey();

        }

        private static TreeLengthDepth SolveForPeak(int x, int y)
        {
            var area = LandScapeMatrix.ReducedLandScape;
            var solution = new TreeLengthDepth { Depth = 0, Length = 0 };

            var leaves = ReturnLeaves(x, y, 1);

            foreach (var leaf in leaves)
            {
                if (leaf.LengthFromRoot > solution.Length)
                {
                    solution.Length = leaf.LengthFromRoot;
                    solution.Depth = area[x, y] - leaf.Z;
                }
                else if (leaf.LengthFromRoot == solution.Length)
                    if (area[x, y] - leaf.Z > solution.Depth)
                    {
                        solution.Depth = area[x, y] - leaf.Z;
                    }
            }

            return solution;
        }

        private static IEnumerable<LandScapeCell> ReturnLeaves(int x, int y, int length)
        {
            var area = LandScapeMatrix.ReducedLandScape;

            var leaves = new List<LandScapeCell>();
            var addParentCell = true;

            //left
            if (x > 0 && area[x - 1, y] < area[x, y])
            {
                leaves.AddRange(ReturnLeaves(x - 1, y, length + 1));
                addParentCell = false;
            }

            //right
            if (x < LandScapeMatrix.SquareMapSide - 1 && area[x + 1, y] < area[x, y])
            {
                leaves.AddRange(ReturnLeaves(x + 1, y, length + 1));
                addParentCell = false;
            }

            //top
            if (y > 0 && (area[x, y - 1] < area[x, y]))
            {
                leaves.AddRange(ReturnLeaves(x, y - 1, length + 1));
                addParentCell = false;
            }

            //bottom
            if (y < LandScapeMatrix.SquareMapSide - 1 && area[x, y + 1] < area[x, y])
            {
                leaves.AddRange(ReturnLeaves(x, y + 1, length + 1));
                addParentCell = false;
            }

            if (addParentCell)
                leaves.Add(new LandScapeCell(x, y, area[x, y], length));

            return leaves;
        }
    }
}

