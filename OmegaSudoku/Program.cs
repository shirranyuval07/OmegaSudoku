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
                SudokuBoard board = new SudokuBoard("530070000600195000098000060800060003400803001700020006060000280000419005000080079\r\n\r\n\r\n\r\n");
                board.PrintBoard();
                Solver.Solve(board);
                Console.WriteLine("after Solving: ");
                board.PrintBoard();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

        }

    }
}
