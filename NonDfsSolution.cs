using System.Collections.Generic;
using System.Linq;

namespace SkiPuzzle
{
    public  class NonDfsSolution
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
                        _solutions.Merge(NonDfs(x, y));

            return _solutions;
        }

        private List<Solution> NonDfs(int x, int y)
        {
            var cells = _landScapeMatrix.Cells;
            var currentCell = _landScapeMatrix.Cells[x, y];
            var possibleSolutions = new List<Solution>();
            var isLeafCell = true;

            if (y > 0 && cells[x, y - 1].Z < cells[x, y].Z)
            {
                _landScapeMatrix.Cells[x, y - 1].IsPeak = false;
                var left = NonDfs(x, y - 1);
                possibleSolutions.AddRange(left.Select(leaf =>
                {
                    leaf.Path.Insert(0, currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            if (y < _landScapeMatrix.MatrixLength - 1 && cells[x, y + 1].Z < cells[x, y].Z)
            {
                _landScapeMatrix.Cells[x, y + 1].IsPeak = false;
                var right = NonDfs(x, y + 1);
                possibleSolutions.AddRange(right.Select(leaf =>
                {
                    leaf.Path.Insert(0, currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            if (x > 0 && cells[x - 1, y].Z < cells[x, y].Z)
            {
                _landScapeMatrix.Cells[x - 1, y].IsPeak = false;
                var top = NonDfs(x - 1, y);
                possibleSolutions.AddRange(top.Select(leaf =>
                {
                    leaf.Path.Insert(0, currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            if (x < _landScapeMatrix.MatrixLength - 1 && cells[x + 1, y].Z < cells[x, y].Z)
            {
                _landScapeMatrix.Cells[x + 1, y].IsPeak = false;
                var bottom = NonDfs(x + 1, y);
                possibleSolutions.AddRange(bottom.Select(leaf =>
                {
                    leaf.Path.Insert(0, currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            if (isLeafCell)
                possibleSolutions.Add(new Solution { Path = new List<LandScapeCell> { currentCell } });

            return possibleSolutions.FindSolutions();
        }
    }
}
