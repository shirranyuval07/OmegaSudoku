using System;
using System.Collections.Generic;
using System.Text;

namespace OmegaSudoku.Exceptions
{
    class InvalidPuzzleException : SudokuException
    {
        public InvalidPuzzleException(string message) : base("Puzzle is invalid: " + message)
        {
        }
    }
}
