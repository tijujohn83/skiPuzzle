using System;
using System.IO;
using System.Text;
using SkiPuzzle.Utils;

namespace SkiPuzzle.Model
{
    public class LandScapeMatrix
    {
        public int MatrixLength { get; }
        public bool GenerateRandomMatrix { get; }

        public LandScapeMatrix()
        {
            MatrixLength = 1000;
            GenerateRandomMatrix = false;
            Init();
        }

        public LandScapeMatrix(int matrixLength, bool generateRandomMatrix)
        {
            MatrixLength = matrixLength;
            GenerateRandomMatrix = generateRandomMatrix;
            Init();
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
            {
                if (File.Exists($"LandScape{MatrixLength}.txt"))
                    _sourceString = File.ReadAllText($"LandScape{MatrixLength}.txt");
                else
                {
                    _sourceString = Utilities.GetCommaSeparatedNumbers(MatrixLength);
                    File.WriteAllText($"LandScape{MatrixLength}.txt", _sourceString, Encoding.UTF8);
                }
            }

            return _sourceString;
        }

        public void ResetMatrix()
        {
            Init();
        }

        public void ResetMatrixData()
        {
            _sourceString = null;
            Init();
        }

        private void Init()
        {
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
        }

        public LandScapeCell[,] Cells
        {
            get
            {
                return _landScape;
            }
        }

    }
}
