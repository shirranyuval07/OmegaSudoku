using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;


namespace OmegaSudoku.Tests.Unit
{
    public class SolverTests
    {
        //testing 17 clue sudoku boards 
        [Fact]
        public void SolveSudokuTest17Clue()
        {
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
                long start = 0;
                long end = 0;
                long elapsedTicks = 0;
                TimeSpan elapsed = TimeSpan.Zero;
                Stopwatch stopwatch = new Stopwatch();
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
                    if(elapsed.TotalSeconds > 1)
                        throw new Exception("TestSolveSudoku failed: Sudoku Took too long");

                }
                catch (Exception ex)
                {
                    // Assert
                    throw new Exception("TestSolveSudoku failed: An exception occurred while solving the puzzle.", ex);
                }
            }
        }
        [Fact]
        public void SolveSudokuTestABunchOf9x9()
        {
            List<string> listPuzzle = new List<string>();
            List<string> listSol = new List<string>();
            int i = 0;
            using (var reader = new StreamReader(@"C:\Users\Owner\Yuval_Omega\OmegaSudoku\OmegaSudoku\bin\Debug\sudoku.csv"))
            {
                while (!reader.EndOfStream) 
                {
                    var line = reader.ReadLine();

                    var strings = line.Split(',');
                    listPuzzle.Add(strings[0]);
                    listSol.Add(strings[1]);
                }
            }

            var puzzleAndSol = listPuzzle.Zip(listSol, (puzzle, sol) => new { puzzle, sol });
            TimeSpan maxTimeSpan = TimeSpan.Zero;
            TimeSpan overall = TimeSpan.Zero;
            string longestPuzzle = "";
            Console.WriteLine();
            long count = 0;
            string puzzle = "";
            string sol = "";
            long start = 0;
            long end = 0;
            long elapsedTicks = 0;
            TimeSpan elapsed = TimeSpan.Zero;
            Stopwatch stopwatch = new Stopwatch();
            foreach (var item in puzzleAndSol)
            {
                puzzle = item.puzzle;
                sol = item.sol;
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
                    if (!board.IsValidBoard())
                        continue;
                    Solver.Solve(board);
                    if (board.ToString() != sol)
                        throw new SudokuException("TestSolveSudoku failed: Sudoku solution is incorrect");
                    end = Stopwatch.GetTimestamp();
                    elapsedTicks = end - start;
                    elapsed = TimeSpan.FromSeconds(elapsedTicks / (double)Stopwatch.Frequency);
                    Console.Write("Number: " + count++ + " ,time is: " + elapsed.ToString(@"mm\:ss\.ffffff") + "\r");
                    overall += elapsed;
                    if (elapsed > maxTimeSpan)
                    {
                        maxTimeSpan = elapsed;
                        longestPuzzle = puzzle;
                    }

                    if (elapsed.TotalSeconds > 1)
                        throw new SudokuException("TestSolveSudoku failed: Sudoku Took too long");

                }
                catch (SudokuException ex)
                {
                    // Assert
                    Console.WriteLine(puzzle);
                    throw new SudokuException("TestSolveSudoku failed: An exception occurred while solving the puzzle.", ex);
                }
            }

            Console.WriteLine("Longest puzzle is {0} \n Run time is: {1}", longestPuzzle, maxTimeSpan);

            Console.WriteLine("Overall time for 1,000,000 puzzles: " + overall.ToString(@"mm\:ss\.ffffff"));

            Console.WriteLine("Average time is: " + overall.TotalMilliseconds / 1000000 + " milliseconds");


            Console.WriteLine("Thank you for using Omega Sudoku Solver!");

        }
    }
}
