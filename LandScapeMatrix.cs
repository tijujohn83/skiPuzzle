using System;
using System.IO;

namespace Problem1
{
    public static class LandScapeMatrix
    {
        public static object LockObj = new object();
        //input assumes area represented as a square matrix of the size below
        public const int SquareMapSide = 2;
        public const bool GenerateRandom = false;

        private static LandScapeCell[,] _landScape;

        private static string _sourceString;

        private static string GetSourceString()
        {
            if (_sourceString != null)
                return _sourceString;

            if (GenerateRandom)
                _sourceString = Utilities.GetCommaSeparatedNumbers(SquareMapSide);
            else
                _sourceString = File.ReadAllText($"..\\..\\LandScape{SquareMapSide}.txt");

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

                    string input = GetSourceString();

                    _landScape = new LandScapeCell[SquareMapSide, SquareMapSide];
                    int row = 0, col = 0;

                    foreach (var item in input.Split(','))
                    {
                        _landScape[row, col] = new LandScapeCell
                        {
                            X = row,
                            Y = col,
                            Z = Convert.ToInt32(item)
                        };

                        col++;
                        if (col == SquareMapSide)
                        {
                            row++;
                            col = 0;
                        }

                        if (row == SquareMapSide)
                            break;
                    }

                    return _landScape;
                }
            }
        }

    }
}
