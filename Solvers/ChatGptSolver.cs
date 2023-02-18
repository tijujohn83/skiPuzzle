using System.Collections.Generic;
using skiPuzzle.Solvers;
using SkiPuzzle.Model;
using SkiPuzzle.Utils;

namespace SkiPuzzle.Solvers
{

    public class DfsSolver : ISolver
    {
        private readonly List<Solution> _solutions = new List<Solution>();
        private LandScapeMatrix _landScapeMatrix;

        public List<Solution> Solve(LandScapeMatrix landScapeMatrix)
        {
            _solutions.Clear(); // Clear the solutions list before solving
            _landScapeMatrix = landScapeMatrix;

            // Use a single loop to traverse the matrix
            for (var i = 0; i < _landScapeMatrix.MatrixLength * _landScapeMatrix.MatrixLength; i++)
            {
                var x = i / _landScapeMatrix.MatrixLength;
                var y = i % _landScapeMatrix.MatrixLength;

                // Check if the cell is not a peak
                if (!_landScapeMatrix.Cells[x, y].IsPeak.HasValue)
                {
                    var cellSolutions = Dfs(x, y);
                    _solutions.AddRange(cellSolutions);
                }
            }

            return _solutions;
        }

        private List<Solution> Dfs(int x, int y)
        {
            var cells = _landScapeMatrix.Cells;
            var currentCell = cells[x, y];
            var solutions = new List<Solution>();
            var isLeafCell = true;

            // Define an array of neighbor cells to traverse
            var neighbors = new (int, int)[] { (x, y - 1), (x, y + 1), (x - 1, y), (x + 1, y) };

            // Traverse each neighbor cell
            foreach (var (nx, ny) in neighbors)
            {
                // Check if the neighbor cell is within bounds
                if (nx >= 0 && nx < _landScapeMatrix.MatrixLength && ny >= 0 && ny < _landScapeMatrix.MatrixLength)
                {
                    var neighborCell = cells[nx, ny];

                    // Check if the neighbor cell is lower than the current cell
                    if (neighborCell.Z < currentCell.Z)
                    {
                        // Mark the neighbor cell as visited
                        _landScapeMatrix.Cells[nx, ny].IsPeak = false;

                        // Recursively traverse the neighbor cell
                        var neighborSolutions = Dfs(nx, ny);

                        // Prepend the current cell to each solution path
                        foreach (var neighborSolution in neighborSolutions)
                        {
                            neighborSolution.Path.Insert(0, currentCell);
                        }

                        // Add the neighbor cell solutions to the current solutions
                        solutions.AddRange(neighborSolutions);
                        isLeafCell = false;
                    }
                }
            }

            // If the current cell is a leaf node, create a solution with just the current cell
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
