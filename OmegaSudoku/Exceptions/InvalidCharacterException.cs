using System;
using System.Collections.Generic;
using System.Text;

namespace OmegaSudoku.Exceptions
{
    class InvalidCharacterException : SudokuException
    {
        public InvalidCharacterException(char value) : base($"Invalid puzzle {value} , that sign sure is cool though!")
        {
        }
        public InvalidCharacterException(char value, int boardLen) : base($"Invalid character '{value}' for Sudoku board of size {boardLen}x{boardLen}.")
        {
        }
    }
}
