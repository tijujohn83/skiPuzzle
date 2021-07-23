using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

//Answer:15-1422
namespace Problem1
{
    class Program
    {
        public const int Space = 2;

        static void Main(string[] args)
        {
            var sb = new StringBuilder();
            PrintProblem(sb);
            var solutions = new List<Solution>();

            ISolution nonDfs = new NonDfsSolution();
            sb.AppendLine(nameof(NonDfsSolution));
            solutions.AddRange(nonDfs.Solve());
            SolutionString(solutions, sb);

            LandScapeMatrix.Reset();
            solutions.Clear();

            ISolution dfs = new DfsSolution();
            sb.AppendLine(nameof(DfsSolution));
            solutions.AddRange(dfs.Solve());
            SolutionString(solutions, sb);

            PrintToFile(sb.ToString());

            //Test();

        }

        private static void Test()
        {
            var iterations = 20;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            watch.Reset();
            watch.Start();
            var nonDfs = new NonDfsSolution();
            for (var i = 0; i < iterations; i++)
            {
                nonDfs.Solve();
                LandScapeMatrix.Reset();
            }

            watch.Stop();
            Console.WriteLine(
                $"{nameof(NonDfsSolution)} executed {iterations} times. Time = {watch.ElapsedMilliseconds}");

            watch.Reset();
            watch.Start();
            var dfs = new DfsSolution();
            for (var i = 0; i < iterations; i++)
            {
                dfs.Solve();
                LandScapeMatrix.Reset();
            }

            watch.Stop();
            Console.WriteLine($"{nameof(DfsSolution)} executed {iterations} times. Time = {watch.ElapsedMilliseconds}");

            Console.ReadKey();
        }

        private static void PrintProblem(StringBuilder sb)
        {
            sb.AppendLine("Input");
            for (var x = 0; x < LandScapeMatrix.SquareMapSide; x++)
            {
                for (var y = 0; y < LandScapeMatrix.SquareMapSide; y++)
                {
                    sb.Append($"{LandScapeMatrix.Cells[x, y].Z}".PadLeft(Space).PadRight(Space + 2));
                }

                sb.AppendLine();
            }

            sb.AppendLine();
        }

        private static void PrintToFile(string result)
        {
            File.WriteAllText(@"..\..\solution.txt", result, Encoding.UTF8);
        }

        private static void SolutionString(List<Solution> solutions, StringBuilder sb)
        {
            sb.AppendLine("Peaks");

            for (var x = 0; x < LandScapeMatrix.SquareMapSide; x++)
            {
                for (var y = 0; y < LandScapeMatrix.SquareMapSide; y++)
                {
                    var isPeak = LandScapeMatrix.Cells[x, y].IsPeak;
                    sb.Append(isPeak != null && isPeak.Value
                        ? LandScapeMatrix.Cells[x, y].Z.ToString().PadLeft(Space).PadRight(Space + 2)
                        : "●".PadLeft(Space).PadRight(Space + 2));
                }
                sb.AppendLine();
            }
            sb.AppendLine();

            sb.Append($"Cells Traversed = {solutions.FirstOrDefault()?.Path.Count ?? 0}, Fall = {solutions.FirstOrDefault()?.Depth ?? 0}")
                .AppendLine().AppendLine();

            var solutionCount = 0;

            foreach (var solution in solutions)
            {
                sb.Append("Solution ").AppendLine(solutionCount++.ToString());

                sb.Append("Cells = ").Append(string.Join("🡢", solution.Path.Select(node => $"[{node.X}, {node.Y}]")))
                    .AppendLine();

                sb.Append("Heights = ")
                    .Append(string.Join("🡢", solution.Path.Select(node => LandScapeMatrix.Cells[node.X, node.Y].Z)))
                    .AppendLine();


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
                sb.AppendLine();
            }

            sb.AppendLine("------------------------------------------------------------------------------------------------------");

        }
    }
}

