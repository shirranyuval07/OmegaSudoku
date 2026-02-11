using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku.Utils
{
    static class Constants
    {
        public static int boardLen = 9; //current board len
        public static int boxLen = 3; // according to the board len,box len
        public static string symbols = "123456789";//the symbols of the grid so it will be more generic
        public static char emptyCell = '0';//instead of using a magic number i put it in a variable

        // Mappings between characters and their corresponding indices
        public static int[] CharToIndex;
        public static char[] IndexToChar;

        /// <summary>
        /// initializes the symbol mappings based on the current board length. (for generic reasons from 4x4 to 25x25)
        /// </summary>
        public static void SetSymbol()
        {
            //check board size
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
            CharToIndex = new int[128]; // 128 should be enough to cover all ASCII characters
            IndexToChar = new char[Constants.boardLen];
            Array.Fill(CharToIndex, -1); // Initialize all entries to -1 to indicate invalid characters
            char c = ' ';
            for (int i = 0; i < symbols.Length; i++)
            {
                c = symbols[i];
                CharToIndex[c] = i;
                IndexToChar[i] = c;
            }
        }


    }
}
