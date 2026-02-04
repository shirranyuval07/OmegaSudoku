using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku.Exceptions
{
    class SudokuException : Exception
    {
        public SudokuException(string message) : base("Sudoku Exception found: " + message)
        {
        }
        public SudokuException(string message, Exception inner) : base("Sudoku Exception found: " + message, inner)
        {
        }
    }
}
