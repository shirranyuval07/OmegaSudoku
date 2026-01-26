using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    class HeapCell
    {
        public SquareCell Cell { get; }
        public int HeapIndex { get; set; }
        public HeapCell(SquareCell cell)
        {
            Cell = cell;
            HeapIndex = -1;
        }
        public int CompareTo(HeapCell other)
        {
            return this.Cell.PossibleCount.CompareTo(other.Cell.PossibleCount);
        }
    }
}
