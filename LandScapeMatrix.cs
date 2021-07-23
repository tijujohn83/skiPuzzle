using System;
using System.IO;

namespace SkiPuzzle
{
    public class LandScapeMatrix
    {
        public int MatrixLength { get; }
        public bool GenerateRandomMatrix { get; }

        public LandScapeMatrix()
        {
            MatrixLength = 20;
            GenerateRandomMatrix = false;
        }

        public LandScapeMatrix(int matrixLength, bool generateRandomMatrix)
        {
            MatrixLength = matrixLength;
            GenerateRandomMatrix = generateRandomMatrix;
        }

        private LandScapeCell[,] _landScape;
        private string _sourceString;

        public string GetSourceString()
        {
            if (_sourceString != null)
                return _sourceString;

            if (GenerateRandomMatrix)
                _sourceString = Utilities.GetCommaSeparatedNumbers(MatrixLength);
            else
                _sourceString = File.ReadAllText($"LandScape{MatrixLength}.txt");

            return _sourceString;
        }

        public void ResetMatrix()
        {
            _landScape = null;
        }

        public void ResetMatrixData()
        {
            _landScape = null;
            _sourceString = null;
        }

        public LandScapeCell[,] Cells
        {
            get
            {
                if (_landScape != null)
                    return _landScape;

                var input = GetSourceString();

                _landScape = new LandScapeCell[MatrixLength, MatrixLength];
                int row = 0, col = 0;

                foreach (var item in input.Split(','))
                {
                    _landScape[row, col] = new LandScapeCell(row, col, Convert.ToInt32(item));

                    col++;
                    if (col == MatrixLength)
                    {
                        row++;
                        col = 0;
                    }

                    if (row == MatrixLength)
                        break;
                }

                return _landScape;
            }
        }

    }
}
