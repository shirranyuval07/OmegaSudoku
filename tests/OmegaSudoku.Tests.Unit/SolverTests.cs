using System;
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
    }
}
