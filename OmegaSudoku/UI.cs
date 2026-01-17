using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
            long start = 0;
            long end = 0;
            long elapsedTicks = 0;
            TimeSpan elapsed = TimeSpan.Zero;
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                Console.WriteLine("give sudoku board");

                string input = Console.ReadLine();
                if (input == "HALAS")
                    break;
                Console.WriteLine(input.Length +" is the input length");
                try
                {

                    SudokuBoard board = new SudokuBoard(input);
                    board.PrintBoard();
                    start = Stopwatch.GetTimestamp();
                    Solver.Solve(board);
                    Console.WriteLine("after Solving: ");
                    board.PrintBoard();
                    end = Stopwatch.GetTimestamp();
                    elapsedTicks = end - start;
                    elapsed = TimeSpan.FromSeconds(elapsedTicks / (double)Stopwatch.Frequency);
                    Console.WriteLine(elapsed.ToString(@"mm\:ss\.ffffff"));

                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
            Console.WriteLine("Thank you for using Omega Sudoku Solver!");

        }

    }
}
