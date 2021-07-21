using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

//Answer:15-1422
namespace Problem1
{
    class Program
    {
        private static readonly object LockObj = new object();
        public static ConcurrentDictionary<string, IEnumerable<LandScapeCell>> Lookup = new ConcurrentDictionary<string, IEnumerable<LandScapeCell>>();
        public const int Space = 2;

        static void Main(string[] args)
        {
            var solution = new TreeLengthDepth { Depth = 0, Length = 0 };

            solution = NonDfs(solution);
            //solution = Dfs(solution);
            PrintSolution(solution);
        }

        private static void PrintSolution(TreeLengthDepth solution)
        {
            var builder = new StringBuilder();

            builder.Append($"Cells Traversed = {solution.Path.Count}, Fall = {solution.Depth}")
                .AppendLine();

            builder.Append("Cells = ").Append(string.Join("🡢", solution.Path.Select(node => $"[{node.X}, {node.Y}]"))).AppendLine();
            builder.Append("Heights = ").Append(string.Join("🡢", solution.Path.Select(node => LandScapeMatrix.Cells[node.X, node.Y].Z))).AppendLine().AppendLine();

            builder.AppendLine("Input");
            for (var x = 0; x < LandScapeMatrix.SquareMapSide; x++)
            {
                for (var y = 0; y < LandScapeMatrix.SquareMapSide; y++)
                {
                    builder.Append($"{LandScapeMatrix.Cells[x, y].Z}".PadLeft(Space).PadRight(Space + 2));
                }
                builder.AppendLine();
            }

            builder.AppendLine().AppendLine("Peaks");
            for (var x = 0; x < LandScapeMatrix.SquareMapSide; x++)
            {
                for (var y = 0; y < LandScapeMatrix.SquareMapSide; y++)
                {
                    builder.Append(LandScapeMatrix.Cells[x, y].IsPeak.Value ? LandScapeMatrix.Cells[x, y].Z.ToString().PadLeft(Space).PadRight(Space + 2) : "●".PadLeft(Space).PadRight(Space + 2));
                }
                builder.AppendLine();
            }

            builder.AppendLine().AppendLine("Solution");

            for (var x = 0; x < LandScapeMatrix.SquareMapSide; x++)
            {
                for (var y = 0; y < LandScapeMatrix.SquareMapSide; y++)
                {
                    var node = solution.Path.FirstOrDefault(p => p.X == x && p.Y == y);
                    if (node != null)
                    {
                        var nodeIndex = solution.Path.IndexOf(node);
                        var isLastCell = nodeIndex == solution.Path.Count - 1;

                        var start = nodeIndex == 0 ? "@" : "";

                        if (!isLastCell)
                        {
                            var next = solution.Path[nodeIndex + 1];

                            if (next.X == node.X && next.Y < node.Y)
                                builder.Append($"{start}🡠".PadLeft(Space + 1).PadRight(Space + 3));
                            else if (next.X == node.X && next.Y > node.Y)
                                builder.Append($"{start}🡢".PadLeft(Space + 1).PadRight(Space + 3));
                            else if (next.X < node.X && next.Y == node.Y)
                                builder.Append($"{start}🡡".PadLeft(Space + 1).PadRight(Space + 3));
                            else if (next.X > node.X && next.Y == node.Y)
                                builder.Append($"{start}🡣".PadLeft(Space + 1).PadRight(Space + 3));
                            else if (next.X < node.X && next.Y < node.Y)
                                builder.Append($"{start}🡤".PadLeft(Space + 1).PadRight(Space + 3));
                            else if (next.X < node.X && next.Y > node.Y)
                                builder.Append($"{start}🡥".PadLeft(Space + 1).PadRight(Space + 3));
                            else if (next.X > node.X && next.Y > node.Y)
                                builder.Append($"{start}🡦".PadLeft(Space + 1).PadRight(Space + 3));
                            else if (next.X > node.X && next.Y < node.Y)
                                builder.Append($"{start}🡧".PadLeft(Space + 1).PadRight(Space + 3));
                        } else
                        {
                            builder.Append("@".PadLeft(Space).PadRight(Space + 2));
                        }
                    } else
                    {
                        builder.Append("●".PadLeft(Space).PadRight(Space + 2));
                    }
                }
                builder.AppendLine();
            }
            builder.AppendLine();

            File.WriteAllText(@"..\..\solution.txt", builder.ToString(), Encoding.UTF8);
        }

        private static TreeLengthDepth Dfs(TreeLengthDepth solution)
        {
            for (var i = 0; i < LandScapeMatrix.SquareMapSide; i++)
            {
                for (var j = 0; j < LandScapeMatrix.SquareMapSide; j++)
                {
                    var x = i;
                    var y = j;
                    SolveForPeaksDfs(x, y, ref solution);
                }
            }
            solution.Path.Reverse();
            return solution;
        }

        private static TreeLengthDepth NonDfs(TreeLengthDepth solution)
        {
            //Parallel.For(0, LandScapeMatrix.SquareMapSide, i =>
            //{
            //    Parallel.For(0, LandScapeMatrix.SquareMapSide, j =>
            //    {
            //        var x = i;
            //        var y = j;
            //        SolveForPeaks(x, y, ref solution);
            //    });
            //});
            //return solution;

            for (var i = 0; i < LandScapeMatrix.SquareMapSide; i++)
            {
                for (var j = 0; j < LandScapeMatrix.SquareMapSide; j++)
                {
                    var x = i;
                    var y = j;
                    SolveForPeaks(x, y, ref solution);
                }
            }

            solution.Path.Reverse();
            return solution;
        }

        private static void SolveForPeaks(int x, int y, ref TreeLengthDepth solution)
        {
            if (LandScapeMatrix.Cells[x, y].IsPeak.HasValue) return;
            var current = LandScapeMatrix.Cells[x, y].Z;

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

        private static void SolveForPeaksDfs(int x, int y, ref TreeLengthDepth solution)
        {
            if (LandScapeMatrix.Cells[x, y].IsPeak.HasValue) return;

            var current = LandScapeMatrix.Cells[x, y].Z;

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

        private static TreeLengthDepth SolveForPeak(int x, int y)
        {
            var landScape = LandScapeMatrix.Cells;
            var leafCells = ReturnAllLeaves(x, y, 1);
            var solutionCell = leafCells.OrderByDescending(cell => cell.LengthFromPeak).ThenBy(cell => cell.Z).FirstOrDefault();
            return new TreeLengthDepth { Depth = landScape[x, y].Z - solutionCell.Z, Length = solutionCell.LengthFromPeak, Path = solutionCell.Path.ToList() };
        }

        private static TreeLengthDepth SolveForPeakDfs(int x, int y)
        {
            var landScape = LandScapeMatrix.Cells;
            var solutionCell = Dfs(x, y, 1);
            return new TreeLengthDepth { Depth = landScape[x, y].Z - solutionCell.Z, Length = solutionCell.LengthFromPeak, Path = solutionCell.Path.ToList() };
        }

        private static LandScapeCell Dfs(int x, int y, int cellsTraversed)
        {
            void Update(LandScapeCell newSol, LandScapeCell currSol)
            {
                if (newSol.LengthFromPeak > currSol.LengthFromPeak)
                {
                    currSol.CopyFrom(newSol);
                } else if (newSol.LengthFromPeak == currSol.LengthFromPeak && newSol.Z < currSol.Z)
                {
                    currSol.CopyFrom(newSol);
                }
            }

            var landScape = LandScapeMatrix.Cells;
            var solutionCell = LandScapeMatrix.Cells[x, y];

            //left
            if (y > 0 && landScape[x, y - 1].Z < landScape[x, y].Z)
            {
                Update(Dfs(x, y - 1, cellsTraversed + 1), solutionCell);
            }

            //right
            if (y < LandScapeMatrix.SquareMapSide - 1 && landScape[x, y + 1].Z < landScape[x, y].Z)
            {
                Update(Dfs(x, y + 1, cellsTraversed + 1), solutionCell);
            }

            //top
            if (x > 0 && landScape[x - 1, y].Z < landScape[x, y].Z)
            {
                Update(Dfs(x - 1, y, cellsTraversed + 1), solutionCell);
            }

            //bottom
            if (x < LandScapeMatrix.SquareMapSide - 1 && landScape[x + 1, y].Z < landScape[x, y].Z)
            {
                Update(Dfs(x + 1, y, cellsTraversed + 1), solutionCell);
            }

            solutionCell.LengthFromPeak = cellsTraversed;
            solutionCell.Path = new List<LandScapeCell> { solutionCell };
            return solutionCell;
        }

        private static IEnumerable<LandScapeCell> ReturnAllLeaves(int x, int y, int cellsTraversed)
        {
            //if (Lookup.TryGetValue(LookupKey(x, y, cellsTraversed), out var cache))
            //{
            //    return cache;
            //}

            var landScape = LandScapeMatrix.Cells;
            var currentCell = LandScapeMatrix.Cells[x, y];
            var leafCells = new List<LandScapeCell>();
            var isLeafCell = true;

            //left
            if (y > 0 && landScape[x, y - 1].Z < landScape[x, y].Z)
            {
                leafCells.AddRange(ReturnAllLeaves(x, y - 1, cellsTraversed + 1).Select(leaf =>
                {
                    leaf.Path.Add(currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            //right
            if (y < LandScapeMatrix.SquareMapSide - 1 && landScape[x, y + 1].Z < landScape[x, y].Z)
            {
                leafCells.AddRange(ReturnAllLeaves(x, y + 1, cellsTraversed + 1).Select(leaf =>
                {
                    leaf.Path.Add(currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            //top
            if (x > 0 && landScape[x - 1, y].Z < landScape[x, y].Z)
            {
                leafCells.AddRange(ReturnAllLeaves(x - 1, y, cellsTraversed + 1).Select(leaf =>
                {
                    leaf.Path.Add(currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            //bottom
            if (x < LandScapeMatrix.SquareMapSide - 1 && landScape[x + 1, y].Z < landScape[x, y].Z)
            {
                leafCells.AddRange(ReturnAllLeaves(x + 1, y, cellsTraversed + 1).Select(leaf =>
                {
                    leaf.Path.Add(currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            if (isLeafCell)
                leafCells.Add(new LandScapeCell(x, y, landScape[x, y].Z, cellsTraversed) { Path = new List<LandScapeCell> { currentCell } });


            //Lookup.TryAdd(LookupKey(x, y, cellsTraversed), leafCells);
            return leafCells;
        }

        public static string LookupKey(int x, int y, int z)
        {
            return x + "-" + y + "-" + z;
        }
    }
}

