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
        public static int MaskForDigit(int d) => 1 << (d - 1);
        public static int LowestBit(int n) => n & -n;
        public static int BitToDigit(int bit)
        {
            int digit = 1;
            while ((bit >>= 1) != 0)
                digit++;
            return digit;
        }
        public static char MaskToChar(int mask)
        {
            int num = (int)Math.Log(mask, 2) + 1;
            return (char)('0' + num);
        }
        public static int BitFromChar(char val) => 1 << (val - '1');

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
    }
}
