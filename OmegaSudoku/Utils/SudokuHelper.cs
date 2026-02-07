using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.Numerics;

namespace OmegaSudoku.Utils
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
            // 1. Check Empty Cell
            if (value == Constants.emptyCell) return;

            // 2. Check Array Bounds (Prevent IndexOutOfRange for weird characters)
            // Constants.CharToIndex is size 128.
            if (value >= Constants.CharToIndex.Length)
            {
                throw new Exceptions.InvalidCharacterException(value);
            }
            int index = Constants.CharToIndex[value];

            // 4. Check Validity
            if (index == -1 || index >= boardLen)
            {
                if (value == 'Ω') // Specific check you had
                    throw new Exceptions.InvalidCharacterException(value);
                else
                    throw new Exceptions.InvalidCharacterException(value, boardLen);
            }
        }

    }
}
