using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

//Answer for Landscape1000.txt: Hops=15, Depth=1422
//Hops=[693, 603]🡢[694, 603]🡢[694, 604]🡢[695, 604]🡢[695, 605]🡢[694, 605]🡢[694, 606]🡢[693, 606]🡢[692, 606]🡢[692, 605]🡢[691, 605]🡢[691, 606]🡢[690, 606]🡢[689, 606]🡢[689, 607]
//HopHeights=1422🡢1412🡢1316🡢1304🡢1207🡢1162🡢965🡢945🡢734🡢429🡢332🡢310🡢214🡢143🡢0

namespace Problem1
{
    class Program
    {
        public const int Space = 2;
        public const bool PrintLandscapeMatrix = false;
        public const bool PrintPeaks = false;
        public const bool PrintSolutionPath = false;

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
            if (!PrintLandscapeMatrix) return;

            sb.AppendLine("Input");
            for (var x = 0; x < LandScapeMatrix.MatrixLength; x++)
            {
                for (var y = 0; y < LandScapeMatrix.MatrixLength; y++)
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
            if (PrintPeaks)
            {
                sb.AppendLine("Peaks");
                for (var x = 0; x < LandScapeMatrix.MatrixLength; x++)
                {
                    for (var y = 0; y < LandScapeMatrix.MatrixLength; y++)
                    {
                        var isPeak = LandScapeMatrix.Cells[x, y].IsPeak;
                        sb.Append(isPeak != null && isPeak.Value
                            ? LandScapeMatrix.Cells[x, y].Z.ToString().PadLeft(Space).PadRight(Space + 2)
                            : "●".PadLeft(Space).PadRight(Space + 2));
                    }
                    sb.AppendLine();
                }
                sb.AppendLine();
            }


            sb.Append($"Hops = {solutions.FirstOrDefault()?.Path.Count ?? 0}, Depth = {solutions.FirstOrDefault()?.Depth ?? 0}")
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


                if (PrintSolutionPath)
                {
                    for (var x = 0; x < LandScapeMatrix.MatrixLength; x++)
                    {
                        for (var y = 0; y < LandScapeMatrix.MatrixLength; y++)
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

            }

            sb.AppendLine("------------------------------------------------------------------------------------------------------");

        }
    }
}

