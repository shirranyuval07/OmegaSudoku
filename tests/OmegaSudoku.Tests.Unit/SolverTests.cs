using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace OmegaSudoku.Tests.Unit
{
    public class SolverTests
    {
        [Fact]

        //testing 1 million sudoku board (takes about 12 minutes)
        public void SolveSudoku_TestSolutions()
        {
            List<string> listA = new List<string>();
            List<string> listB = new List<string>();
            
            using (var reader = new StreamReader(@"C:\Users\Owner\Yuval_Omega\OmegaSudoku\OmegaSudoku\bin\Debug\sudoku.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    listA.Add(values[0]);
                    listB.Add(values[1]);
                }
            }
            var combined = listA.Zip(listB, (n, w) => new { Puzzle = n, Solution = w });

            foreach (var puzzNsol in combined)
            {
                // Arrange
                string puzzle = puzzNsol.Puzzle;
                SudokuBoard board = new SudokuBoard(puzzle);
                // Act
                Solver.Solve(board);
                // Assert
                string expectedSolution = puzzNsol.Solution;
                if (board.ToString() != expectedSolution)
                {
                    throw new Exception("TestSolveSudoku failed: The solved board does not match the expected solution.");
                }
            }
        }

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
                
                try
                {
                    // Arrange
                    SudokuBoard board = new SudokuBoard(puzzle);
                    // Act
                    Solver.Solve(board);
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
