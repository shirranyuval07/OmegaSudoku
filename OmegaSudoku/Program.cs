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
            SudokuBoard board = new SudokuBoard("800000070006010053040600000000080400003000700020005038000000800004050061900002000");
            board.PrintBoard();

        }

    }
}
