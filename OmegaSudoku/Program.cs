using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Omega Sudoku!");
            SudokuBoard board = new SudokuBoard("023456789400000000500000000600000000700000000800000000900000000100000000300000000\r\n\r\n\r\n");
            board.PrintBoard();

        }

    }
}
