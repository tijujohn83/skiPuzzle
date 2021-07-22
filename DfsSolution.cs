namespace Problem1
{
    public class DfsSolution : ISolution
    {
        public Solution Solve()
        {
            var solution = new Solution();

            for (var x = 0; x < LandScapeMatrix.SquareMapSide; x++)
            {
                for (var y = 0; y < LandScapeMatrix.SquareMapSide; y++)
                {
                    SolveForPeaksDfs(x, y, ref solution);
                }
            }

            return solution;
        }

        private static void SolveForPeaksDfs(int x, int y, ref Solution solution)
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
                var solutionThisPeak = SolveForPeakDfs(x, y);

                //longest then steepest
                if (solutionThisPeak.Length > solution.Length)
                {
                    solution = solutionThisPeak;
                } else if (solutionThisPeak.Length == solution.Length && solutionThisPeak.Depth > solution.Depth)
                {
                    solution = solutionThisPeak;
                }
            }

        }

        private static Solution SolveForPeakDfs(int x, int y)
        {
            return Dfs(x, y);
        }

        private static Solution Dfs(int x, int y)
        {
            var solution = new Solution();
            var landScape = LandScapeMatrix.Cells;
            var currentCell = landScape[x, y];
            var isLeafCell = true;

            Solution BestSolution(Solution sol1, Solution sol2)
            {
                if (sol1.Length > sol2.Length)
                {
                    return sol1;
                }
                if (sol1.Length == sol2.Length && sol1.Depth > sol2.Depth)
                {
                    return sol1;
                }
                return sol2;
            }

            //left
            if (y > 0 && landScape[x, y - 1].Z < currentCell.Z)
            {
                var dfs = Dfs(x, y - 1);
                dfs.Path.Insert(0, currentCell);
                solution = dfs;
                isLeafCell = false;
            }

            //right
            if (y < LandScapeMatrix.SquareMapSide - 1 && landScape[x, y + 1].Z < currentCell.Z)
            {
                var dfs = Dfs(x, y + 1);
                dfs.Path.Insert(0, currentCell);
                solution = BestSolution(solution, dfs);
                isLeafCell = false;
            }

            //top
            if (x > 0 && landScape[x - 1, y].Z < currentCell.Z)
            {
                var dfs = Dfs(x - 1, y);
                dfs.Path.Insert(0, currentCell);
                solution = BestSolution(solution, dfs);
                isLeafCell = false;
            }

            //bottom
            if (x < LandScapeMatrix.SquareMapSide - 1 && landScape[x + 1, y].Z < currentCell.Z)
            {
                var dfs = Dfs(x + 1, y);
                dfs.Path.Insert(0, currentCell);
                solution = BestSolution(solution, dfs);
                isLeafCell = false;
            }

            if (isLeafCell)
                solution.Path.Add(currentCell);

            return solution;
        }
    }
}
