using System;
using System.Collections.Generic;
using System.Text;

namespace OmegaSudoku.Exceptions
{
    class UnsolvableSudokuException : SudokuException
    {
        public UnsolvableSudokuException(string message) : base("Board is unsolvable  " + message)
        {
        }
    }
}
