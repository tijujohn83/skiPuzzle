using System;
using System.IO;

namespace Problem1
{
    public static class LandScapeMatrix
    {
        public static object LockObj = new object();
        public const int MatrixLength = 100;
        public const bool GenerateRandomMatrix = false;

        private static LandScapeCell[,] _landScape;

        private static string _sourceString;

        private static string GetSourceString()
        {
            if (_sourceString != null)
                return _sourceString;

            if (GenerateRandomMatrix)
                _sourceString = Utilities.GetCommaSeparatedNumbers(MatrixLength);
            else
                _sourceString = File.ReadAllText($"..\\..\\LandScape{MatrixLength}.txt");

            return _sourceString;
        }

        public static void Reset()
        {
            _landScape = null;
        }

        public static LandScapeCell[,] Cells
        {
            get
            {
                lock (LockObj)
                {
                    if (_landScape != null)
                        return _landScape;

                    var input = GetSourceString();

                    _landScape = new LandScapeCell[MatrixLength, MatrixLength];
                    int row = 0, col = 0;

                    foreach (var item in input.Split(','))
                    {
                        _landScape[row, col] = new LandScapeCell(row,col,Convert.ToInt32(item));

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
}
