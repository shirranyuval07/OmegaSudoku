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
            try
            {
                SudokuBoard board = new SudokuBoard("800000070006010053040600000000080400003000700020005038000000800004050061900002000\r\n\r\n\r\n\r\n");
                board.PrintBoard();
                Solver.BackTrack(board);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

        }

    }
}
