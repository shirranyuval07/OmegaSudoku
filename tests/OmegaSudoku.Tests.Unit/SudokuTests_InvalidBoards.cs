using OmegaSudoku.Core;
using OmegaSudoku.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OmegaSudoku.Tests.Unit
{
    public class SudokuTests_InvalidBoards
    {
        [Fact]
        public void TestInvalidBoards()
        {
            string empty = "";
            string nullString = null;
            string rowDuplicate = "110000000200000000300000000400000000500000000600000000700000000800000000900000000";
            string colDuplicate = "100000000100000000200000000300000000400000000500000000600000000700000000800000000";
            List<string> invalidPuzzles = new List<string> { empty,nullString,rowDuplicate, colDuplicate };
            foreach (var puzzle in invalidPuzzles)
            {
                try
                {
                    // Attempt to create the board
                    SudokuBoard board = new SudokuBoard(puzzle);

                    // If we get here without an exception, fail the test
                    throw new Exception("TestInvalidBoards failed: Expected SudokuException was not thrown for an invalid board.");
                }
                catch (SudokuException ex)
                {
                    // Assert
                    // We check if the message indicates the board is invalid (adjust expected string to match your exact exception message)
                    Assert.Contains("Sudoku Exception found", ex.Message);
                    Assert.Contains("Puzzle is invalid", ex.Message);
                }
            }
        }

    }
}
