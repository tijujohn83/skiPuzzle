using System;
using System.IO;

namespace Problem1
{
    public static class LandScapeMatrix
    {
        //input assumes area represented as a square matrix of the size below
        public const int SquareMapSide = 1000;

        private static int[,] _landScape;
        public static int MinHeight = int.MaxValue;
        public static int MaxHeight = int.MinValue;
        
        public static int[,] LandScape
        {
            get
            {
                if (_landScape != null) return _landScape;

                var input = File.ReadAllText(@"..\..\LandScape.txt");
                _landScape = new int[SquareMapSide, SquareMapSide];
                int row = 0, column = 0;

                foreach (var item in input.Split(','))
                {
                    _landScape[row, column] = Convert.ToInt32(item);

                    if (_landScape[row, column] < MinHeight)
                        MinHeight = _landScape[row, column];

                    if (_landScape[row, column] > MaxHeight)
                        MaxHeight = _landScape[row, column];

                    row++;
                    if (row == SquareMapSide)
                    {
                        column++;
                        row = 0;
                    }
                    if (column == SquareMapSide)
                        break;
                }

                return _landScape;
            }
        }

    }
}
