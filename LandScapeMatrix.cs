using System;
using System.IO;

namespace Problem1
{
    public static class LandScapeMatrix
    {
        public static object LockObj = new object();
        //input assumes area represented as a square matrix of the size below
        public const int SquareMapSide = 3;
        public const bool GenerateRandom = false;

        private static LandScapeCell[,] _landScape;

        public static LandScapeCell[,] Cells
        {
            get
            {
                lock (LockObj)
                {
                    if (_landScape != null)
                        return _landScape;

                    string input;

                    if (GenerateRandom)
                        input = Utilities.GetCommaSeparatedNumbers(SquareMapSide);
                    else
                        input = File.ReadAllText($"..\\..\\LandScape{SquareMapSide}.txt");


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
