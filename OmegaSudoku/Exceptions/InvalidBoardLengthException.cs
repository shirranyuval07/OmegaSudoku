using System;
using System.Collections.Generic;
using System.Text;

namespace OmegaSudoku.Exceptions
{
    class InvalidBoardLengthException : SudokuException
    {
        public InvalidBoardLengthException(string message) : base("Board Length is invalid: " + message)
        {
        }
    }
}
