using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.Numerics;

namespace OmegaSudoku
{
    static class SudokuHelper
    {
        public static int LowestBit(int n) => n & -n;
        public static int BitToIndex(int mask)
        {
            return BitOperations.TrailingZeroCount((uint)mask);
        }
        public static int ClearLowestBit(int n) => n &=  n - 1;

        public static int ClearBit(int n,int mask) => n &= ~mask;

        public static int AddBit(int n, int bit) => n |= bit;
        
        public static char MaskToChar(int mask)
        {
            return Constants.IndexToChar[BitToIndex(mask)];
        }
        public static int BitFromChar(char value) => 1 << Constants.CharToIndex[value];


        //counting number of bits that are "on"
        public static int CountBits(int n)
        {
            return BitOperations.PopCount((uint)n);
        }

        public static void ValidateChar(char value, int boardLen)
        {
            if (value != Constants.emptyCell && (!Constants.CharToIndex.ContainsKey(value) || Constants.CharToIndex[value] >= boardLen))
            {
                if (value == 'Ω')
                    throw new InvalidPuzzleException($"Invalid puzzle {value} , that sign sure is cool though!");
                else
                    throw new InvalidPuzzleException($"Invalid character '{value}' for Sudoku board of size {boardLen}x{boardLen}.");
            }
        }

    }
}
