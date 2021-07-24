using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using skiPuzzle.Solvers;
using SkiPuzzle.Model;
using SkiPuzzle.Utils;

namespace SkiPuzzle.Solvers
{
    public class ParallelDfsSolver : ISolver
    {
        private List<Solution> _solutions;
        private LandScapeMatrix _landScapeMatrix;

        public List<Solution> Solve(LandScapeMatrix landScapeMatrix)
        {
            var finalSolutions1 = new List<Solution>();
            var finalSolutions2 = new List<Solution>();

            _landScapeMatrix = landScapeMatrix;

            var task1 = Task.Run(() =>
            {
                for (int x = 0; x < _landScapeMatrix.MatrixLength / 2; x++)
                    for (int y = 0; y < _landScapeMatrix.MatrixLength; y++)
                        finalSolutions1.Merge(Dfs(x, y));

            });

            var task2 = Task.Run(() =>
            {
                for (int x = _landScapeMatrix.MatrixLength / 2; x < _landScapeMatrix.MatrixLength; x++)
                    for (int y = 0; y < _landScapeMatrix.MatrixLength; y++)
                        finalSolutions2.Merge(Dfs(x, y));
            });

            Task.WaitAll(new Task[] { task1, task2 });
            finalSolutions1.Merge(finalSolutions2);

            return finalSolutions1;
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
