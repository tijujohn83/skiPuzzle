﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Answer:15-1422
namespace Problem1
{
    class Program
    {
        private static readonly object LockObj = new object();
        public const int Space = 2;

        static void Main(string[] args)
        {
            var sb = new StringBuilder();

            sb.AppendLine(nameof(NonDfs));
            var solution = NonDfs();
            SolutionString(solution, sb);

            sb.AppendLine(nameof(Dfs));
            solution = Dfs();
            SolutionString(solution, sb);

            PrintToFile(sb.ToString());
        }

        private static void PrintToFile(string result)
        {
            File.WriteAllText(@"..\..\solution.txt", result, Encoding.UTF8);
        }

        private static void SolutionString(TreeLengthDepth solution, StringBuilder sb)
        {
            sb.Append($"Cells Traversed = {solution.Path.Count}, Fall = {solution.Depth}")
                .AppendLine();

            sb.Append("Cells = ").Append(string.Join("🡢", solution.Path.Select(node => $"[{node.X}, {node.Y}]"))).AppendLine();
            sb.Append("Heights = ").Append(string.Join("🡢", solution.Path.Select(node => LandScapeMatrix.Cells[node.X, node.Y].Z))).AppendLine().AppendLine();

            sb.AppendLine("Input");
            for (var x = 0; x < LandScapeMatrix.SquareMapSide; x++)
            {
                for (var y = 0; y < LandScapeMatrix.SquareMapSide; y++)
                {
                    sb.Append($"{LandScapeMatrix.Cells[x, y].Z}".PadLeft(Space).PadRight(Space + 2));
                }
                sb.AppendLine();
            }

            sb.AppendLine().AppendLine("Peaks");
            for (var x = 0; x < LandScapeMatrix.SquareMapSide; x++)
            {
                for (var y = 0; y < LandScapeMatrix.SquareMapSide; y++)
                {
                    var isPeak = LandScapeMatrix.Cells[x, y].IsPeak;
                    sb.Append(isPeak != null && isPeak.Value ? LandScapeMatrix.Cells[x, y].Z.ToString().PadLeft(Space).PadRight(Space + 2) : "●".PadLeft(Space).PadRight(Space + 2));
                }
                sb.AppendLine();
            }

            sb.AppendLine().AppendLine("Solution");

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
                                sb.Append($"{start}🡠".PadLeft(Space + 1).PadRight(Space + 3));
                            else if (next.X == node.X && next.Y > node.Y)
                                sb.Append($"{start}🡢".PadLeft(Space + 1).PadRight(Space + 3));
                            else if (next.X < node.X && next.Y == node.Y)
                                sb.Append($"{start}🡡".PadLeft(Space + 1).PadRight(Space + 3));
                            else if (next.X > node.X && next.Y == node.Y)
                                sb.Append($"{start}🡣".PadLeft(Space + 1).PadRight(Space + 3));
                            else if (next.X < node.X && next.Y < node.Y)
                                sb.Append($"{start}🡤".PadLeft(Space + 1).PadRight(Space + 3));
                            else if (next.X < node.X && next.Y > node.Y)
                                sb.Append($"{start}🡥".PadLeft(Space + 1).PadRight(Space + 3));
                            else if (next.X > node.X && next.Y > node.Y)
                                sb.Append($"{start}🡦".PadLeft(Space + 1).PadRight(Space + 3));
                            else if (next.X > node.X && next.Y < node.Y)
                                sb.Append($"{start}🡧".PadLeft(Space + 1).PadRight(Space + 3));
                        } else
                        {
                            sb.Append("@".PadLeft(Space).PadRight(Space + 2));
                        }
                    } else
                    {
                        sb.Append("●".PadLeft(Space).PadRight(Space + 2));
                    }
                }
                sb.AppendLine();
            }
            sb.AppendLine("------------------------------------------------------------------------------------------------------");

        }

        private static TreeLengthDepth Dfs()
        {
            var solution = new TreeLengthDepth { Depth = 0, Length = 0, Path = new List<LandScapeCell>() };

            for (var x = 0; x < LandScapeMatrix.SquareMapSide; x++)
            {
                for (var y = 0; y < LandScapeMatrix.SquareMapSide; y++)
                {
                    SolveForPeaksDfs(x, y, solution);
                }
            }
            solution.Path.Reverse();
            return solution;
        }

        private static TreeLengthDepth NonDfs()
        {
            var solution = new TreeLengthDepth { Depth = 0, Length = 0, Path = new List<LandScapeCell>() };

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

        private static void SolveForPeaks(int x, int y, TreeLengthDepth solution)
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

        private static void SolveForPeaksDfs(int x, int y, TreeLengthDepth solution)
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
            var solutionCell = leafCells.OrderByDescending(cell => cell.CellsTraversed).ThenBy(cell => cell.Z).FirstOrDefault();
            return new TreeLengthDepth { Depth = landScape[x, y].Z - solutionCell.Z, Length = solutionCell.CellsTraversed, Path = solutionCell.Path.ToList() };
        }

        private static TreeLengthDepth SolveForPeakDfs(int x, int y)
        {
            var landScape = LandScapeMatrix.Cells;
            var solutionCell = Dfs(x, y, 1);
            return new TreeLengthDepth { Depth = landScape[x, y].Z - solutionCell.Z, Length = solutionCell.CellsTraversed, Path = solutionCell.Path.ToList() };
        }

        private static LandScapeCell Dfs(int x, int y, int cellsTraversed)
        {

            var landScape = LandScapeMatrix.Cells;
            var solutionCell = LandScapeMatrix.Cells[x, y];
            solutionCell.CellsTraversed = cellsTraversed;
            solutionCell.Path = solutionCell.Path ?? new List<LandScapeCell>{solutionCell};

            LandScapeCell FindSolution(LandScapeCell newSol, LandScapeCell currSol)
            {
                if (newSol.CellsTraversed > currSol.CellsTraversed)
                {
                    newSol.Path.Add(solutionCell);
                    return newSol;
                }

                if (newSol.CellsTraversed == currSol.CellsTraversed && newSol.Z < currSol.Z)
                { 
                    newSol.Path.Add(solutionCell);
                    return newSol;
                }

                return currSol;
            }
            

            //left
            if (y > 0 && landScape[x, y - 1].Z < landScape[x, y].Z)
            {
                solutionCell = FindSolution(Dfs(x, y - 1, cellsTraversed + 1), solutionCell);
            }

            //right
            if (y < LandScapeMatrix.SquareMapSide - 1 && landScape[x, y + 1].Z < landScape[x, y].Z)
            {
                solutionCell = FindSolution(Dfs(x, y + 1, cellsTraversed + 1), solutionCell);
            }

            //top
            if (x > 0 && landScape[x - 1, y].Z < landScape[x, y].Z)
            {
                solutionCell = FindSolution(Dfs(x - 1, y, cellsTraversed + 1), solutionCell);
            }

            //bottom
            if (x < LandScapeMatrix.SquareMapSide - 1 && landScape[x + 1, y].Z < landScape[x, y].Z)
            {
                solutionCell = FindSolution(Dfs(x + 1, y, cellsTraversed + 1), solutionCell);
            }

            return solutionCell;
        }

        private static IEnumerable<LandScapeCell> ReturnAllLeaves(int x, int y, int cellsTraversed)
        {
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


            return leafCells;
        }

    }
}

