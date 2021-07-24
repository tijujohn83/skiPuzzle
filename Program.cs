using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SkiPuzzle.Model;
using SkiPuzzle.Solvers;

//Answer for LandScape1000_original.txt: Hops=15, Depth=1422
//Hops=[693, 603]🡢[694, 603]🡢[694, 604]🡢[695, 604]🡢[695, 605]🡢[694, 605]🡢[694, 606]🡢[693, 606]🡢[692, 606]🡢[692, 605]🡢[691, 605]🡢[691, 606]🡢[690, 606]🡢[689, 606]🡢[689, 607]
//HopHeights=1422🡢1412🡢1316🡢1304🡢1207🡢1162🡢965🡢945🡢734🡢429🡢332🡢310🡢214🡢143🡢0

namespace SkiPuzzle
{
    class Program
    {
        private static readonly object Lock = new object();

        public static int Space = 2;
        public static bool PrintInputMatrix = false;
        public static bool PrintPeaks = false;
        public static bool PrintSolutionPath = false;
        public static bool RunFindMatricesWithMultipleSolutions = false;
        public static bool RunPerformanceTest = false;

        static void Main(string[] args)
        {
            if (args.Contains(nameof(PrintInputMatrix)))
                PrintInputMatrix = true;
            if (args.Contains(nameof(PrintPeaks)))
                PrintPeaks = true;
            if (args.Contains(nameof(PrintSolutionPath)))
                PrintSolutionPath = true;
            if (args.Contains(nameof(RunFindMatricesWithMultipleSolutions)))
                RunFindMatricesWithMultipleSolutions = true;
            if (args.Contains(nameof(RunPerformanceTest)))
                RunPerformanceTest = true;

            var sb = new StringBuilder();
            var landScapeMatrix = new LandScapeMatrix();

            if (RunFindMatricesWithMultipleSolutions)
                FindMatricesWithMultipleSolutions();

            if (PrintInputMatrix)
                PrintMatrix(landScapeMatrix, sb);

            if (RunPerformanceTest)
                TestPerformance(landScapeMatrix, sb);

            if (!RunPerformanceTest)
            {
                sb.AppendLine(nameof(BruteForceSolver));
                SolutionString(landScapeMatrix, new BruteForceSolver().Solve(landScapeMatrix), sb);
                landScapeMatrix.ResetMatrix();

                sb.AppendLine(nameof(DfsSolver));
                SolutionString(landScapeMatrix, new DfsSolver().Solve(landScapeMatrix), sb);
                landScapeMatrix.ResetMatrix();

                sb.AppendLine(nameof(ParallelDfsSolver));
                SolutionString(landScapeMatrix, new ParallelDfsSolver().Solve(landScapeMatrix), sb);
                landScapeMatrix.ResetMatrix();

            }
            PrintToFile(sb.ToString());
        }

        private static void FindMatricesWithMultipleSolutions()
        {
            var trial = 1;
            var bestCount = 0;

            LandScapeMatrix bestMatrix = null;
            var sb = new StringBuilder();

            Parallel.For((long)0, 10000000, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (_, state) =>
            {
                var dfs = new DfsSolver();
                var landScapeMatrixParallel = new LandScapeMatrix();

                var sol = dfs.Solve(landScapeMatrixParallel);
                Interlocked.Increment(ref trial);

                lock (Lock)
                {
                    bestMatrix = bestMatrix ?? landScapeMatrixParallel;
                    if (sol.Count > bestCount)
                    {
                        bestMatrix = landScapeMatrixParallel;
                        bestCount = sol.Count;
                        Console.WriteLine(trial + "-" + sol.Count);
                        File.WriteAllText($"LandScape{bestMatrix.MatrixLength}.txt", bestMatrix.GetSourceString(), Encoding.UTF8);
                        sb.Clear();
                        SolutionString(bestMatrix, sol, sb);
                        PrintToFile(sb.ToString());
                    }
                }

            });
        }

        private static void TestPerformance(LandScapeMatrix landScapeMatrix, StringBuilder sb)
        {
            var iterations = 10;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            Console.WriteLine($"Running {nameof(BruteForceSolver)}");
            watch.Reset();
            watch.Start();
            for (var i = 0; i < iterations; i++)
            {
                new BruteForceSolver().Solve(landScapeMatrix);
                landScapeMatrix.ResetMatrix();
            }
            watch.Stop();
            Console.WriteLine($"Done {nameof(BruteForceSolver)}");
            sb.AppendLine($"{nameof(BruteForceSolver)} executed {iterations} times. Time = {watch.ElapsedMilliseconds}");


            Console.WriteLine($"Running {nameof(DfsSolver)}");
            watch.Reset();
            watch.Start();
            for (var i = 0; i < iterations; i++)
            {
                new DfsSolver().Solve(landScapeMatrix);
                landScapeMatrix.ResetMatrix();
            }
            watch.Stop();
            Console.WriteLine($"Done {nameof(DfsSolver)}");
            sb.AppendLine($"{nameof(DfsSolver)} executed {iterations} times. Time = {watch.ElapsedMilliseconds}");


            Console.WriteLine($"Running {nameof(ParallelDfsSolver)}");
            watch.Reset();
            watch.Start();
            for (var i = 0; i < iterations; i++)
            {
                new ParallelDfsSolver().Solve(landScapeMatrix);
                landScapeMatrix.ResetMatrix();
            }
            watch.Stop();
            sb.AppendLine($"{nameof(ParallelDfsSolver)} executed {iterations} times. Time = {watch.ElapsedMilliseconds}");
            Console.WriteLine($"Done {nameof(ParallelDfsSolver)}");

            sb.AppendLine("----------------------Exiting Test----------------------");
        }

        private static void PrintMatrix(LandScapeMatrix landScapeMatrix, StringBuilder sb)
        {
            sb.AppendLine("Input");
            for (var x = 0; x < landScapeMatrix.MatrixLength; x++)
            {
                for (var y = 0; y < landScapeMatrix.MatrixLength; y++)
                {
                    sb.Append($"{landScapeMatrix.Cells[x, y].Z}".PadLeft(Space).PadRight(Space + 2));
                }

                sb.AppendLine();
            }
            sb.AppendLine();
        }

        private static void PrintToFile(string result)
        {
            File.WriteAllText(@"solution.txt", result, Encoding.UTF8);
        }

        private static void SolutionString(LandScapeMatrix landScapeMatrix, List<Solution> solutions, StringBuilder sb)
        {
            if (PrintPeaks)
            {
                sb.AppendLine("Peaks");
                for (var x = 0; x < landScapeMatrix.MatrixLength; x++)
                {
                    for (var y = 0; y < landScapeMatrix.MatrixLength; y++)
                    {
                        var isPeak = !landScapeMatrix.Cells[x, y].IsPeak.HasValue;
                        sb.Append(isPeak
                                    ? landScapeMatrix.Cells[x, y].Z.ToString().PadLeft(Space).PadRight(Space + 2)
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
                sb.Append("Solution ").AppendLine((++solutionCount).ToString());

                sb.Append("Cells = ").Append(string.Join("🡢", solution.Path.Select(node => $"[{node.X}, {node.Y}]")))
                    .AppendLine();

                sb.Append("Heights = ")
                    .Append(string.Join("🡢", solution.Path.Select(node => landScapeMatrix.Cells[node.X, node.Y].Z)))
                    .AppendLine().AppendLine();


                if (PrintSolutionPath)
                {
                    for (var x = 0; x < landScapeMatrix.MatrixLength; x++)
                    {
                        for (var y = 0; y < landScapeMatrix.MatrixLength; y++)
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
                                }
                                else
                                {
                                    sb.Append("@".PadLeft(Space).PadRight(Space + 2));
                                }
                            }
                            else
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

