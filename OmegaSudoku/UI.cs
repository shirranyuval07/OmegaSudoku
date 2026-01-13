using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    static class UI
    {
        public static void StartSudokuSolver()
        {
            Console.WriteLine("Welcome to Omega Sudoku! \n enter HALAS to stop");
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                Console.WriteLine("give sudoku board");

                string input = Console.ReadLine();
                if (input == "HALAS")
                    break;
                Console.WriteLine(input.Length);
                try
                {

                    SudokuBoard board = new SudokuBoard(input);
                    board.PrintBoard();
                    long start = Stopwatch.GetTimestamp();
                    Solver.Solve(board);
                    Console.WriteLine("after Solving: ");
                    board.PrintBoard();
                    long end = Stopwatch.GetTimestamp();
                    long elapsedTicks = end - start;
                    TimeSpan elapsed = TimeSpan.FromSeconds(elapsedTicks / (double)Stopwatch.Frequency);
                    Console.WriteLine(elapsed.ToString(@"mm\:ss\.ffffff"));

                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
                finally
                {
                    Console.WriteLine("Thank you for using Omega Sudoku Solver!");
                }
            }


        }
    }
}
