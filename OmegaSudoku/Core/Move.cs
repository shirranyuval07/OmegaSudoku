using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku.Core
{
    readonly struct Move
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
