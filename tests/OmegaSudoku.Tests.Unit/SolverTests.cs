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
                    throw new SudokuException("TestSolveSudoku failed: An exception occurred while solving the puzzle.", ex);
                }
            }
        }
        [Fact]
        public void TestUnsolvableBoard()
        {
            string puzzle = "123000000456000000000090000000000000000000000000000000000000000000000000000000000";
            string puzzle2 = "000020003000000000000100000100000030006000000007000500039000000020010000050000001";
            string puzzle3 = "000000051260000000008600000000071020140050000000000300000300400500900000700000000";
            string puzzle4 = "400800502208400973000002840796500000000673019532980000070200195600105000000390400";
            string puzzle5 = "000010500604000000000005000000300062510400000700000000052000100000008030000600000";
            try
            {
                for(int i = 0; i < 5;i++)
                {
                    string currentPuzzle = i switch
                    {
                        0 => puzzle,
                        1 => puzzle2,
                        2 => puzzle3,
                        3 => puzzle4,
                        4 => puzzle5,
                        _ => throw new SudokuException("Invalid puzzle index.")
                    };
                    // Arrange
                    SudokuBoard board = new SudokuBoard(currentPuzzle);
                    // Act
                    bool solved = Solver.Solve(board);
                    if (!solved)
                        throw new SudokuException("The Sudoku puzzle is unsolvable.");
                }
            }
            catch (SudokuException ex)
            {
                // Assert
                Assert.Equal("The Sudoku puzzle is unsolvable.", ex.Message);
                return;
            }
            throw new Exception("TestUnsolvableBoard failed: Expected SudokuException was not thrown.");
        }

    }
}
