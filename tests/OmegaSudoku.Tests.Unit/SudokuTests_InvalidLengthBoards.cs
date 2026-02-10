using OmegaSudoku.Core;
using OmegaSudoku.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OmegaSudoku.Tests.Unit
{
    public class SudokuTests_InvalidLengthBoards
    {
        [Fact]
        public void TestInvalidLengthBoards()
        {
            string lengthone = "1";
            string shortString = "12345";
            string longString = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            List<string> wrongLengthPuzzles = new List<string> {lengthone, shortString, longString };
            foreach (var puzzle in wrongLengthPuzzles)
            {
                try
                {
                    SudokuBoard board = new SudokuBoard(puzzle);
                    throw new Exception($"TestInvalidLengthBoards failed: Input length was {puzzle.Length}, expected SudokuException.");
                }
                catch (SudokuException ex)
                {
                    // Assert
                    Assert.Contains("Sudoku Exception found", ex.Message);
                    Assert.Contains("Board Length is invalid:", ex.Message);
                }
            }
        }

    }
}
