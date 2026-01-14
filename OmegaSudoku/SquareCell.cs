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

        private HashSet<char> possibleValues;

        private char value { get; set; }
        public SquareCell(int row, int col, char value)
        {
            this.row = row;
            this.col = col;
            this.value = value;
            this.possibleValues = new HashSet<char>();
        }
        public SquareCell(SquareCell cell)
        {
            this.row = cell.row;
            this.col = cell.col;
            this.value = cell.value;
            this.possibleValues = new HashSet<char>(cell.possibleValues);
        }
        public void SetPossibleValues(HashSet<char> values)
        {
            this.possibleValues = values;
        }

        public void RemovePossibleValue(char val)
        {
            if (val != '0') return;
            this.possibleValues.Remove(val);
        }

        public HashSet<char> PossibleValues
        {
            get { return possibleValues; }
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
            return "(" + row + "," + col + ")" +" And its value is: "+value;
        }
        public bool Failed()
        {
            return possibleValues.Count == 0;
        }
    }
}