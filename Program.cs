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
            ISolution nonDfs = new NonDfsSolution();

            sb.AppendLine(nameof(NonDfsSolution));
            var solution = nonDfs.Solve();
            SolutionString(solution, sb);

            LandScapeMatrix.Reset();

            ISolution dfs = new DfsSolution();

            sb.AppendLine(nameof(DfsSolution));
            solution = dfs.Solve();
            SolutionString(solution, sb);

            PrintToFile(sb.ToString());
        }

        private static void PrintToFile(string result)
        {
            File.WriteAllText(@"..\..\solution.txt", result, Encoding.UTF8);
        }

        private static void SolutionString(Solution solution, StringBuilder sb)
        {
            sb.Append($"Cells Traversed = {solution.LongestPath.Count}, Fall = {solution.Depth}")
                .AppendLine();

            sb.Append("Cells = ").Append(string.Join("🡢", solution.LongestPath.Select(node => $"[{node.X}, {node.Y}]"))).AppendLine();
            sb.Append("Heights = ").Append(string.Join("🡢", solution.LongestPath.Select(node => LandScapeMatrix.Cells[node.X, node.Y].Z))).AppendLine().AppendLine();

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
                    var node = solution.LongestPath.FirstOrDefault(p => p.X == x && p.Y == y);
                    if (node != null)
                    {
                        var nodeIndex = solution.LongestPath.IndexOf(node);
                        var isLastCell = nodeIndex == solution.LongestPath.Count - 1;

                        var start = nodeIndex == 0 ? "@" : "";

                        if (!isLastCell)
                        {
                            var next = solution.LongestPath[nodeIndex + 1];

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

    }
}

