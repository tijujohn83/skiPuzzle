using System;
using System.Collections.Generic;

namespace SkiPuzzle
{
    public static class Utilities
    {
        public static string GetCommaSeparatedNumbers(int size)
        {
            var numbers = new List<int>();
            var rnd = new Random();

            for (int i = 0; i < size * size; i++)
            {
                numbers.Add(rnd.Next(0, 1000));
            }

            return string.Join(", ", numbers);
        }

    }
}
