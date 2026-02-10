using System;
using System.Collections.Generic;
using System.Text;
using OmegaSudoku.Core;
using OmegaSudoku.Exceptions;

namespace OmegaSudoku.Tests.Unit
{
    public class SudokuTests_InvalidCharacter
    {
        [Fact]
        public void Sudoku_WithInvalidCharacter()
        {
            string puzzleWithChar = "X00000000000000000000000000000000000000000000000000000000000000000000000000000000";
            string puzzleWithSymbol = "123$00000000000000000000000000000000000000000000000000000000000000000000000000000";

            List<string> badCharPuzzles = new List<string> { puzzleWithChar, puzzleWithSymbol };

            foreach (var puzzle in badCharPuzzles)
            {
                try
                {
                    SudokuBoard board = new SudokuBoard(puzzle);
                    throw new Exception("Sudoku_WithInvalidCharacter failed: Expected SudokuException was not thrown.");
                }
                catch (SudokuException ex)
                {
                    // Assert
                    Assert.Contains("Sudoku Exception found", ex.Message);
                    Assert.Contains("Invalid character", ex.Message);
                }
            }
        }
    }
}
