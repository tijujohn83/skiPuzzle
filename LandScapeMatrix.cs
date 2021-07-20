using System;
using System.IO;

namespace Problem1
{
    public static class LandScapeMatrix
    {
        //input assumes area represented as a square matrix of the size below
        public const int SquareMapSide = 1000;

        private static LandScapeCell[,] _landScape;
        
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
                    _landScape[row, column] = new LandScapeCell
                    {
                        Z = Convert.ToInt32(item)
                    };

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
