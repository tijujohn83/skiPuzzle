using System.Collections.Generic;

namespace Problem1
{
    public static class DfsSolution
    {
        private static List<Solution> _solutions;

        public static List<Solution> Solve()
        {
            _solutions = new List<Solution>();

            for (var x = 0; x < LandScapeMatrix.MatrixLength; x++)
                for (var y = 0; y < LandScapeMatrix.MatrixLength; y++)
                    if (!LandScapeMatrix.Cells[x, y].IsPeak.HasValue)
                        _solutions.Merge(Dfs(x, y));

            return _solutions;
        }

        private static List<Solution> Dfs(int x, int y)
        {
            var landScape = LandScapeMatrix.Cells;
            var currentCell = landScape[x, y];
            var solutions = new List<Solution>();
            var isLeafCell = true;

            if (y > 0 && landScape[x, y - 1].Z < currentCell.Z)
            {
                LandScapeMatrix.Cells[x, y - 1].IsPeak = false;
                var left = Dfs(x, y - 1);
                left.ForEach(sol =>
                {
                    sol.Path.Insert(0, currentCell);
                });
                solutions.Merge(left);
                isLeafCell = false;
            }

            if (y < LandScapeMatrix.MatrixLength - 1 && landScape[x, y + 1].Z < currentCell.Z)
            {
                LandScapeMatrix.Cells[x, y + 1].IsPeak = false;
                var right = Dfs(x, y + 1);
                right.ForEach(sol =>
                {
                    sol.Path.Insert(0, currentCell);
                });
                solutions.Merge(right);
                isLeafCell = false;
            }

            if (x > 0 && landScape[x - 1, y].Z < currentCell.Z)
            {
                LandScapeMatrix.Cells[x - 1, y].IsPeak = false;
                var top = Dfs(x - 1, y);
                top.ForEach(sol =>
                {
                    sol.Path.Insert(0, currentCell);
                });
                solutions.Merge(top);
                isLeafCell = false;
            }

            if (x < LandScapeMatrix.MatrixLength - 1 && landScape[x + 1, y].Z < currentCell.Z)
            {
                LandScapeMatrix.Cells[x + 1, y].IsPeak = false;
                var bottom = Dfs(x + 1, y);
                bottom.ForEach(sol =>
                {
                    sol.Path.Insert(0, currentCell);
                });
                solutions.Merge(bottom);
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
