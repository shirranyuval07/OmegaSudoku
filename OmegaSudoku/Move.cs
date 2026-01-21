using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    class Move
    {
        public SquareCell Cell { get; }
        public int PreviousMask { get; }

        public Move(SquareCell cell, int previousMask)
        {
            Cell = cell;
            PreviousMask = previousMask;
        }
    }

}
