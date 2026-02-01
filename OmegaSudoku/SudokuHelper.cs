using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    static class SudokuHelper
    {
        public static int LowestBit(int n) => n & -n;
        public static int LowestBitIndex(int mask)
        {
            int bit = LowestBit(mask);
            int index = 0;
            while(bit > 1)
            {
                bit >>= 1;
                index++;
            }
            return index;
        }
        public static int ClearLowestBit(int n) => n &=  n - 1;

        public static int ClearBit(int n,int mask) => n &= ~mask;

        public static int AddBit(int n, int bit) => n |= bit;
        
        public static int IndexFromBit(int bit)
        {
            int index = 0;
            while (bit > 1)
            {
                bit >>= 1;
                index++;
            }
            return index;
        }
        public static char MaskToChar(int mask)
        {
            int index = LowestBitIndex(mask);
            return Constants.IndexToChar[index];
        }
        public static int BitFromChar(char value) => 1 << Constants.CharToIndex[value];


        //counting number of bits that are "on"
        public static int CountBits(int n)
        {
            int count = 0;
            while (n > 0)
            {
                n &= (n - 1);
                count++;
            }
            return count;
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
