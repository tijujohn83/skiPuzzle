using System;
using System.IO;

namespace Problem1
{
    public static class LandScapeMatrix
    {
        //input assumes area represented as a square matrix of the size below
        public const int SquareMapSide = 1000;

        private static LandScapeCell[,] _landScape;
        public static int MinHeight = int.MaxValue;
        public static int MaxHeight = int.MinValue;
        
        public static LandScapeCell[,] Cells
        {
            get
            {
                if (_landScape != null) return _landScape;

                var input = File.ReadAllText(@"..\..\LandScape.txt");
                _landScape = new LandScapeCell[SquareMapSide, SquareMapSide];
                int row = 0, column = 0;

                foreach (var item in input.Split(','))
                {
                    _landScape[row, column].Z = Convert.ToInt32(item);

                    if (_landScape[row, column].Z < MinHeight)
                        MinHeight = _landScape[row, column].Z;

                    if (_landScape[row, column].Z > MaxHeight)
                        MaxHeight = _landScape[row, column].Z;

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
