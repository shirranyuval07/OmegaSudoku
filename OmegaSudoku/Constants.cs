using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    static class Constants
    {
        public static int boardLen = 9;
        public static int boxLen = 3;
        public static string symbols = "123456789";
        public static char emptyCell = '0';

        public static Dictionary<char, int> CharToIndex;
        public static Dictionary<int, char> IndexToChar;


        public static void SetSymbol()
        {
            if (boardLen == 4)
            {
                symbols = "1234";
                boxLen = 2;
            }
            if (boardLen == 9)
            {
                symbols =  "123456789";
                boxLen = 3;
            }
            else if (boardLen == 16)
            {
                symbols = "123456789ABCDEFG";
                boxLen = 4;
            }
            else if (boardLen == 25)
            {
                symbols = "123456789ABCDEFGHIJKLMNOP";
                boxLen = 5;
            }
            CharToIndex = new Dictionary<char, int>();
            IndexToChar = new Dictionary<int, char>();

            for (int i = 0; i < symbols.Length; i++)
            {
                CharToIndex[symbols[i]] = i;
                IndexToChar[i] = symbols[i];
            }
        }


    }
}
