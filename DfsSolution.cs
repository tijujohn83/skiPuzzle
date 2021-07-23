using System.Collections.Generic;

namespace Problem1
{
    public class DfsSolution
    {
        private List<Solution> _solutions;
        private LandScapeMatrix _landScapeMatrix;

        public List<Solution> Solve(LandScapeMatrix landScapeMatrix)
        {
            _solutions = new List<Solution>();
            _landScapeMatrix = landScapeMatrix;

            for (var x = 0; x < _landScapeMatrix.MatrixLength; x++)
                for (var y = 0; y < _landScapeMatrix.MatrixLength; y++)
                    if (!_landScapeMatrix.Cells[x, y].IsPeak.HasValue)
                        _solutions.Merge(Dfs(x, y));

            return _solutions;
        }

        private List<Solution> Dfs(int x, int y)
        {
            var cells = _landScapeMatrix.Cells;
            var currentCell = cells[x, y];
            var solutions = new List<Solution>();
            var isLeafCell = true;

            if (y > 0 && cells[x, y - 1].Z < currentCell.Z)
            {
                _landScapeMatrix.Cells[x, y - 1].IsPeak = false;
                var left = Dfs(x, y - 1);
                left.ForEach(sol =>
                {
                    sol.Path.Insert(0, currentCell);
                });
                solutions.Merge(left);
                isLeafCell = false;
            }

            if (y < _landScapeMatrix.MatrixLength - 1 && cells[x, y + 1].Z < currentCell.Z)
            {
                _landScapeMatrix.Cells[x, y + 1].IsPeak = false;
                var right = Dfs(x, y + 1);
                right.ForEach(sol =>
                {
                    sol.Path.Insert(0, currentCell);
                });
                solutions.Merge(right);
                isLeafCell = false;
            }

            if (x > 0 && cells[x - 1, y].Z < currentCell.Z)
            {
                _landScapeMatrix.Cells[x - 1, y].IsPeak = false;
                var top = Dfs(x - 1, y);
                top.ForEach(sol =>
                {
                    sol.Path.Insert(0, currentCell);
                });
                solutions.Merge(top);
                isLeafCell = false;
            }

            if (x < _landScapeMatrix.MatrixLength - 1 && cells[x + 1, y].Z < currentCell.Z)
            {
                _landScapeMatrix.Cells[x + 1, y].IsPeak = false;
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
