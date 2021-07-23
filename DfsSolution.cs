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

           
            LandScapeCell leftCell;
            if (y - 1 < 0)
                leftCell = currentCell;
            else
            {
                leftCell = LandScapeMatrix.Cells[x, y - 1];
                if (currentCell.Z > leftCell.Z) leftCell.IsPeak = false;
            }

            LandScapeCell rightCell;
            if (y + 1 >= LandScapeMatrix.MatrixLength)
                rightCell = currentCell;
            else
            {
                rightCell = LandScapeMatrix.Cells[x, y + 1];
                if (currentCell.Z > rightCell.Z) rightCell.IsPeak = false;
            }

            LandScapeCell topCell;
            if (x - 1 < 0)
                topCell = currentCell;
            else
            {
                topCell = LandScapeMatrix.Cells[x - 1, y];
                if (currentCell.Z > topCell.Z) topCell.IsPeak = false;
            }

            LandScapeCell bottomCell;
            if (x + 1 >= LandScapeMatrix.MatrixLength)
                bottomCell = currentCell;
            else
            {
                bottomCell = LandScapeMatrix.Cells[x + 1, y];
                if (currentCell.Z > bottomCell.Z) bottomCell.IsPeak = false;
            }

            currentCell.IsPeak = currentCell.Z >= leftCell.Z
                                 && currentCell.Z >= rightCell.Z
                                 && currentCell.Z >= topCell.Z
                                 && currentCell.Z >= bottomCell.Z;


            if (currentCell.IsPeak.Value)
                solutions.TakeBestSolutions(Dfs(x, y));

        }

        private static List<Solution> Dfs(int x, int y)
        {
            var solutions = new List<Solution>();
            var landScape = LandScapeMatrix.Cells;
            var currentCell = landScape[x, y];
            var isLeafCell = true;

            if (y > 0 && landScape[x, y - 1].Z < currentCell.Z)
            {
                var left = Dfs(x, y - 1);
                left.ForEach(sol =>
                {
                    sol.Path.Insert(0, currentCell);
                });
                solutions.TakeBestSolutions(left);
                isLeafCell = false;
            }

            if (y < LandScapeMatrix.MatrixLength - 1 && landScape[x, y + 1].Z < currentCell.Z)
            {
                var right = Dfs(x, y + 1);
                right.ForEach(sol =>
                {
                    sol.Path.Insert(0, currentCell);
                });
                solutions.TakeBestSolutions(right);
                isLeafCell = false;
            }

            if (x > 0 && landScape[x - 1, y].Z < currentCell.Z)
            {
                var top = Dfs(x - 1, y);
                top.ForEach(sol =>
                {
                    sol.Path.Insert(0, currentCell);
                });
                solutions.TakeBestSolutions(top);
                isLeafCell = false;
            }

            if (x < LandScapeMatrix.MatrixLength - 1 && landScape[x + 1, y].Z < currentCell.Z)
            {
                var bottom = Dfs(x + 1, y);
                bottom.ForEach(sol =>
                {
                    sol.Path.Insert(0, currentCell);
                });
                solutions.TakeBestSolutions(bottom);
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
