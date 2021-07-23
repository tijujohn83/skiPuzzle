using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//Answer for LandScape1000_original.txt: Hops=15, Depth=1422
//Hops=[693, 603]🡢[694, 603]🡢[694, 604]🡢[695, 604]🡢[695, 605]🡢[694, 605]🡢[694, 606]🡢[693, 606]🡢[692, 606]🡢[692, 605]🡢[691, 605]🡢[691, 606]🡢[690, 606]🡢[689, 606]🡢[689, 607]
//HopHeights=1422🡢1412🡢1316🡢1304🡢1207🡢1162🡢965🡢945🡢734🡢429🡢332🡢310🡢214🡢143🡢0

namespace SkiPuzzle
{
    class Program
    {
        private static readonly object Lock = new object();

        public const int Space = 2;
        public const bool PrintLandscapeMatrix = false;
        public const bool PrintPeaks = true;
        public const bool PrintSolutionPath = false;
        public const bool RunFindMatricesWithMultipleSolutions = false;
        public const bool PerformanceTest = false;

        static void Main(string[] args)
        {
            FindMatricesWithMultipleSolutions();

            var sb = new StringBuilder();
            var landScapeMatrix = new LandScapeMatrix();
            PrintMatrix(landScapeMatrix, sb);

            sb.AppendLine(nameof(NonDfsSolution));
            SolutionString(landScapeMatrix, new NonDfsSolution().Solve(landScapeMatrix), sb);
            landScapeMatrix.ResetMatrix();

            sb.AppendLine(nameof(DfsSolution));
            SolutionString(landScapeMatrix, new DfsSolution().Solve(landScapeMatrix), sb);


            PrintToFile(sb.ToString());

            TestPerformance(landScapeMatrix);

        }

        private static void FindMatricesWithMultipleSolutions()
        {
            if (!RunFindMatricesWithMultipleSolutions) return;

            var trial = 1;
            var bestCount = 0;

            LandScapeMatrix bestMatrix = null;
            var sb = new StringBuilder();

            Parallel.For((long)0, 10000000, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (_, state) =>
            {
                var dfs = new DfsSolution();
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

        private static void TestPerformance(LandScapeMatrix landScapeMatrix)
        {
            if (!PerformanceTest) return;

            var iterations = 20;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            watch.Reset();
            watch.Start();
            for (var i = 0; i < iterations; i++)
            {
                new NonDfsSolution().Solve(landScapeMatrix);
                landScapeMatrix.ResetMatrix();
            }

            watch.Stop();
            Console.WriteLine(
                $"{nameof(NonDfsSolution)} executed {iterations} times. Time = {watch.ElapsedMilliseconds}");

            watch.Reset();
            watch.Start();
            for (var i = 0; i < iterations; i++)
            {
                new DfsSolution().Solve(landScapeMatrix);
                landScapeMatrix.ResetMatrix();
            }

            watch.Stop();
            Console.WriteLine($"{nameof(DfsSolution)} executed {iterations} times. Time = {watch.ElapsedMilliseconds}");

            Console.ReadKey();
        }

        private static void PrintMatrix(LandScapeMatrix landScapeMatrix, StringBuilder sb)
        {
            if (!PrintLandscapeMatrix) return;

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
                        var isPeak = landScapeMatrix.Cells[x, y].IsPeak;
                        sb.Append(isPeak != null && isPeak.Value
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

