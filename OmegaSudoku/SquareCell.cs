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
            get { return possibleMask; }
            set { possibleMask = value; }
        }

        private Tuple<int, int>[] neighbors;

        private char value { get; set; }
        public SquareCell(int row, int col, char value)
        {
            this.row = row;
            this.col = col;
            this.value = value;
            this.possibleMask = value == Constants.emptyCell? (1 << Constants.boardLen) - 1: 0;
            InitializeNeighbors();

        }
        public SquareCell(SquareCell cell)
        {
            this.row = cell.row;
            this.col = cell.col;
            this.value = cell.value;
            this.possibleMask = cell.possibleMask;
            InitializeNeighbors();
        }
        public void InitializeNeighbors()
        {
            var neighbors = new HashSet<Tuple<int, int>>();

            for (int row = 0; row < Constants.boardLen; row++)
            {
                if (row != this.row)
                    neighbors.Add(new Tuple<int, int>(row, this.col));

            }
            for (int col = 0; col < Constants.boardLen; col++)
            {
                if (col != this.col)
                    neighbors.Add(new Tuple<int, int>(this.row, col));
            }
            int boxRowStart = (this.row / Constants.boxLen) * Constants.boxLen;
            int boxColStart = (this.col / Constants.boxLen) * Constants.boxLen;
            for (int r = boxRowStart; r < boxRowStart + Constants.boxLen; r++)
            {
                for (int c = boxColStart; c < boxColStart + Constants.boxLen; c++)
                {
                    if (r != this.row || c != this.col)
                        neighbors.Add(new Tuple<int, int>(r, c));
                }
            }
            this.neighbors = neighbors.ToArray();
        }

        public IEnumerable<char> PossibleValues
        {
            get
            {
                int mask = possibleMask;
                while (mask != 0)
                {
                    int bit = SudokuHelper.LowestBit(mask);
                    yield return SudokuHelper.MaskToChar(bit);
                    mask &= mask - 1;
                }
            }
        }

        /*public void SetPossibleValues(IEnumerable<char> values)
        {
            possibleMask = 0;
            foreach (char v in values)
            {
                if (v != Constants.emptyCell)
                    possibleMask |= 1 << (v - '1');
            }
        }
        */


        public bool RemovePossibleValue(char val)
        {
            if (val == Constants.emptyCell) return false;

            int bit = 1 << (val - '1');
            if ((possibleMask & bit) == 0) return false;

            possibleMask &= ~bit;
            return true;
        }


        public bool AddPossibleValue(char val)
        {
            if (val == Constants.emptyCell) return false;

            int bit = SudokuHelper.BitFromChar(val);
            bool changed = (possibleMask & bit) == 0;

            possibleMask |= bit;
            return changed;
        }
        public int PossibleCount => SudokuHelper.CountBits(possibleMask);

        public bool Contains(char val)
        {
            int bit = 1 << (val - '1');
            return (possibleMask & bit) != 0;
        }

        public Tuple<int, int>[] GetNeighbors()
        {
            return this.neighbors;
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


    }
}