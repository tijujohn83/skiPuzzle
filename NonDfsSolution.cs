using System.Collections.Generic;
using System.Linq;

namespace Problem1
{
    public static class NonDfsSolution
    {
        private static List<Solution> _solutions;

        public static IEnumerable<Solution> Solve()
        {
            _solutions = new List<Solution>();

            for (var x = 0; x < LandScapeMatrix.MatrixLength; x++)
                for (var y = 0; y < LandScapeMatrix.MatrixLength; y++)
                    if (!LandScapeMatrix.Cells[x, y].IsPeak.HasValue)
                        _solutions.Merge(NonDfs(x, y));

            return _solutions;
        }

        private static List<Solution> NonDfs(int x, int y)
        {
            var landScape = LandScapeMatrix.Cells;
            var currentCell = LandScapeMatrix.Cells[x, y];
            var possibleSolutions = new List<Solution>();
            var isLeafCell = true;

            if (y > 0 && landScape[x, y - 1].Z < landScape[x, y].Z)
            {
                LandScapeMatrix.Cells[x, y - 1].IsPeak = false;
                var left = NonDfs(x, y - 1);
                possibleSolutions.AddRange(left.Select(leaf =>
                {
                    leaf.Path.Insert(0, currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            if (y < LandScapeMatrix.MatrixLength - 1 && landScape[x, y + 1].Z < landScape[x, y].Z)
            {
                LandScapeMatrix.Cells[x, y + 1].IsPeak = false;
                var right = NonDfs(x, y + 1);
                possibleSolutions.AddRange(right.Select(leaf =>
                {
                    leaf.Path.Insert(0, currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            if (x > 0 && landScape[x - 1, y].Z < landScape[x, y].Z)
            {
                LandScapeMatrix.Cells[x - 1, y].IsPeak = false;
                var top = NonDfs(x - 1, y);
                possibleSolutions.AddRange(top.Select(leaf =>
                {
                    leaf.Path.Insert(0, currentCell);
                    return leaf;
                }));
                isLeafCell = false;
            }

            if (x < LandScapeMatrix.MatrixLength - 1 && landScape[x + 1, y].Z < landScape[x, y].Z)
            {
                LandScapeMatrix.Cells[x + 1, y].IsPeak = false;
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
