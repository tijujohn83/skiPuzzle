using System.Collections.Generic;

namespace Problem1
{
    public static class DfsSolution
    {
        public static IEnumerable<Solution> Solve()
        {
            var solutions = new List<Solution>();

            for (var x = 0; x < LandScapeMatrix.MatrixLength; x++)
                for (var y = 0; y < LandScapeMatrix.MatrixLength; y++)
                    SolveForCell(x, y, solutions);

            return solutions;
        }

        private static void SolveForCell(int x, int y, List<Solution> solutions)
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
            if (y + 1 >= LandScapeMatrix.MatrixLength)
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
            if (x + 1 >= LandScapeMatrix.MatrixLength)
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
                solutions.TakeBestSolutions(Dfs(x, y));

        }

        private static List<Solution> Dfs(int x, int y)
        {
            var solutions = new List<Solution>();
            var landScape = LandScapeMatrix.Cells;
            var currentCell = landScape[x, y];
            var isLeafCell = true;

            //left
            if (y > 0 && landScape[x, y - 1].Z < currentCell.Z)
            {
                var dfs = Dfs(x, y - 1);
                dfs.ForEach(sol =>
                {
                    sol.Path.Insert(0, currentCell);
                });
                solutions.TakeBestSolutions(dfs);
                isLeafCell = false;
            }

            //right
            if (y < LandScapeMatrix.MatrixLength - 1 && landScape[x, y + 1].Z < currentCell.Z)
            {
                var dfs = Dfs(x, y + 1);
                dfs.ForEach(sol =>
                {
                    sol.Path.Insert(0, currentCell);
                });
                solutions.TakeBestSolutions(dfs);
                isLeafCell = false;
            }

            //top
            if (x > 0 && landScape[x - 1, y].Z < currentCell.Z)
            {
                var dfs = Dfs(x - 1, y);
                dfs.ForEach(sol =>
                {
                    sol.Path.Insert(0, currentCell);
                });
                solutions.TakeBestSolutions(dfs);
                isLeafCell = false;
            }

            //bottom
            if (x < LandScapeMatrix.MatrixLength - 1 && landScape[x + 1, y].Z < currentCell.Z)
            {
                var dfs = Dfs(x + 1, y);
                dfs.ForEach(sol =>
                {
                    sol.Path.Insert(0, currentCell);
                });
                solutions.TakeBestSolutions(dfs);
                isLeafCell = false;
            }

            if (isLeafCell)
            {
                var curSol = new Solution();
                curSol.Path.Add(currentCell);
                solutions.Add(curSol);
            }

            return solutions;
        }

        
    }
}
