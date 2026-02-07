using OmegaSudoku.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaSudoku.Core
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

        public int BoxIndex { get; private set; }


        private char value { get; set; }
        /// <summary>
        /// Initializes a new instance of the SquareCell class with the specified row, column, and cell value.
        /// </summary>
        /// <param name="row">The zero-based row index of the cell within the board.</param>
        /// <param name="col">The zero-based column index of the cell within the board.</param>
        /// <param name="value">The character value to assign to the cell. Typically represents the cell's current state or content.</param>
        public SquareCell(int row, int col, char value)
        {
            this.row = row;
            this.col = col;
            this.value = value;
            this.possibleMask = value == Constants.emptyCell? (1 << Constants.boardLen) - 1: 0;
            this.BoxIndex = (row / Constants.boxLen) * Constants.boxLen + (col / Constants.boxLen);
        }

        

        /// <summary>
        /// Removes the specified value from the set of possible values for this cell.
        /// </summary>
        /// <remarks>If the specified value is not currently a possible value or represents an empty cell,
        /// the method does not modify the set and returns false.</remarks>
        /// <param name="value">The value to remove from the set of possible values. Must not be the designated empty cell character.</param>
        /// <returns>true if the value was a possible value and was removed; otherwise, false.</returns>
        public bool RemovePossibleValue(char value)
        {
            bool flag = true;
            if (value == Constants.emptyCell) flag = false;

            int bit = SudokuHelper.BitFromChar(value);
            if ((possibleMask & bit) == 0) flag = false;

            possibleMask = SudokuHelper.ClearBit(possibleMask,bit);
            PossibleCount--;
            return flag;
        }



        /// <summary>
        /// Determines whether the set contains the specified Sudoku value.
        /// </summary>
        /// <param name="value">The character representing the Sudoku value to locate in the set.</param>
        /// <returns>true if the set contains the specified value; otherwise, false.</returns>
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
        /// <summary>
        /// Determines whether the current state represents a failure condition.
        /// </summary>
        /// <returns><see langword="true"/> if the current state is considered failed; otherwise, <see langword="false"/>.</returns>
        public bool Failed()
        {
            return possibleMask == 0;
        }

       

    }
}