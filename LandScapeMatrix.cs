using System;
using System.IO;

namespace Problem1
{
    public static class LandScapeMatrix
    {
        //input assumes area represented as a square matrix of the size below
        public const int SquareMapSide = 1000;

        private static int[,] _reducedLandScape;
        public static int MinHeight = int.MaxValue;
        public static int MaxHeight = int.MinValue;
        
        public static int[,] ReducedLandScape
        {
            get
            {
                if (_reducedLandScape != null) return _reducedLandScape;

                var input = File.ReadAllText(@"..\..\LandScape.txt");
                _reducedLandScape = new int[SquareMapSide, SquareMapSide];
                int row = 0, column = 0;

                foreach (var item in input.Split(','))
                {
                    _reducedLandScape[row, column] = Convert.ToInt32(item);

                    if (_reducedLandScape[row, column] < MinHeight)
                        MinHeight = _reducedLandScape[row, column];

                    if (_reducedLandScape[row, column] > MaxHeight)
                        MaxHeight = _reducedLandScape[row, column];

                    row++;
                    if (row == SquareMapSide)
                    {
                        column++;
                        row = 0;
                    }
                    if (column == SquareMapSide)
                        break;
                }

                return _reducedLandScape;
            }
        }

    }
}
