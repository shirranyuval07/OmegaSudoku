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
            int count = 0;
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
            List<string> listA = new List<string>();

            using (var reader = new StreamReader(@"C:\Users\Owner\Yuval_Omega\OmegaSudoku\OmegaSudoku\bin\Debug\17_clue.txt"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    listA.Add(line);
                }
            }

            foreach (string puzzle in listA)
            {
                start = 0;
                end = 0;
                elapsedTicks = 0;
                elapsed = TimeSpan.Zero;
                stopwatch = new Stopwatch();
                try
                {
                    start = Stopwatch.GetTimestamp();
                    // Arrange
                    SudokuBoard board = new SudokuBoard(puzzle);
                    // Act
                    Solver.Solve(board);
                    end = Stopwatch.GetTimestamp();
                    elapsedTicks = end - start;
                    elapsed = TimeSpan.FromSeconds(elapsedTicks / (double)Stopwatch.Frequency);
                    Console.WriteLine(count++);
                    if (elapsed.TotalSeconds > 1)
                        throw new Exception("TestSolveSudoku failed: Sudoku Took too long");

                }
                catch (Exception ex)
                {
                    // Assert
                    throw new Exception("TestSolveSudoku failed: An exception occurred while solving the puzzle.", ex);
                }
            }
            Console.WriteLine("Thank you for using Omega Sudoku Solver!");

        }

    }
}
