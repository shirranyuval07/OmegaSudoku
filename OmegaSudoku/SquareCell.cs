using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku
{
    class SquareCell
    {
        private int row { get; set; }
        private int col { get; set; }



        private int possibleMask;

        public int PossibleMask
        {
            get => possibleMask;
            set
            {
                possibleMask = value;
                PossibleCount = SudokuHelper.CountBits(possibleMask);
            }
        }

        public int PossibleCount { get;  set; }


        public SquareCell[] Neighbors { get; set; }


        private char value { get; set; }
        public SquareCell(int row, int col, char value)
        {
            this.row = row;
            this.col = col;
            this.value = value;
            this.possibleMask = value == Constants.emptyCell? (1 << Constants.boardLen) - 1: 0;
           // InitializeNeighbors();

        }
        public SquareCell(SquareCell cell)
        {
            this.row = cell.row;
            this.col = cell.col;
            this.value = cell.value;
            this.possibleMask = cell.possibleMask;
           // InitializeNeighbors();
        }

        


        public bool RemovePossibleValue(char value)
        {
            if (value == Constants.emptyCell) return false;

            int bit = SudokuHelper.BitFromChar(value);
            if ((possibleMask & bit) == 0) return false;

            possibleMask = SudokuHelper.ClearBit(possibleMask,bit);
            PossibleCount--;
            return true;
        }


        public bool AddPossibleValue(char val)
        {
            if (val == Constants.emptyCell) return false;

            int bit = SudokuHelper.BitFromChar(val);
            bool changed = (possibleMask & bit) == 0;

            possibleMask = SudokuHelper.AddBit(possibleMask,bit);
            if (changed) PossibleCount++;
            return changed;
        }

        public bool Contains(char value)
        {
            int bit = SudokuHelper.BitFromChar(value);
            return (possibleMask & bit) != 0;
        }


        public int Row
        {
            get { return row; }
            set { row = value; }
        }
        public int Col
        {
            get { return col; }
            set { col = value; }
        }
        public char Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public override string ToString()
        {
            return "(" + row + "," + col + ")" + " And its value is: " + value;
        }
        public bool Failed()
        {
            return possibleMask == 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is SquareCell other)
                return Row == other.Row && Col == other.Col;
            return false;
        }

        public override int GetHashCode()
        {
            return Row * 31 + Col;
        }

    }
}