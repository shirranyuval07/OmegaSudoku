using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    class SudokuException : Exception
    {
        public SudokuException(string message) : base(message)
        {
        }
        public SudokuException(string message, Exception inner) : base(message, inner)
        {
        }
    }
    class InvalidPuzzleException : SudokuException
    {
        public InvalidPuzzleException(string message) : base(message)
        {
        }
        public InvalidPuzzleException(string message, Exception inner) : base(message, inner)
        {
        }
    }

}
